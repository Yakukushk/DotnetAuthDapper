using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using DotnetAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dataContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ReusableSQL _reusableSQL;
        public AuthController(IConfiguration config)
        {
            _dataContext = new Data.DataContextDapper(config);
            _config = config;
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<RegistrationDto, UserComplete>()));
            _reusableSQL = new ReusableSQL(_config);
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(RegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dataContext.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = AuthHelper.GetPasswordHash(userForRegistration.Password, passwordSalt, _config);

                    string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth  ([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('" + userForRegistration.Email +
                        "', @PasswordHash, @PasswordSalt)";



                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dataContext.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {

                        string sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES (" +
                                "'" + userForRegistration.FirstName +
                                "', '" + userForRegistration.LastName +
                                "', '" + userForRegistration.Email +
                                "', '" + userForRegistration.Gender +
                                "', 1)";
                        if (_dataContext.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        return BadRequest("Failed to add user.");
                    }
                    return BadRequest("Failed to register user.");
                }
                return BadRequest("User with this email already exists!");
            }
            return NotFound("Passwords do not match!");
        }

        [AllowAnonymous]
        [HttpPost("RegisterComplete")]
        public IActionResult RegisterComplete(RegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dataContext.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {

                    var userForLogin = new LoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password,
                    };
                    if (AuthHelper.SetPassword(userForLogin, _config))
                    {
                        #region
                        //string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                        //     @FirstName = '" + userForRegistration.FirstName +
                        //     "', @LastName = '" + userForRegistration.LastName +
                        //     "', @Email = '" + userForRegistration.Email +
                        //     "', @Gender = '" + userForRegistration.Gender +
                        //     "', @Active = 1" +
                        //     ", @JobTitle = '" + userForRegistration.JobTitle +
                        //     "', @Department = '" + userForRegistration.Department +
                        //     "', @Salary = '" + userForRegistration.Salary + "'";



                        //if (_dataContext.ExecuteSql(sqlAddUser))
                        //{
                        //    return Ok();
                        //}
                        #endregion

                        var mapUserEntity = _mapper.Map<UserComplete>(userForRegistration);
                        if (_reusableSQL.UpsertUser(mapUserEntity))
                        {
                            return Ok();
                        }
                        return BadRequest("Failed to add user.");
                    }
                    return BadRequest("Failed to register user.");
                }
                return BadRequest("User with this email already exists!");
            }
            return NotFound("Passwords do not match!");
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                     loginDto.Email + "'";


            LoginConfirmationDto existingUsers = _dataContext
                .LoadSingleData<LoginConfirmationDto>(sqlCheckUserExists);

            byte[] passwordHash = AuthHelper.GetPasswordHash(loginDto.Password, existingUsers.PasswordSalt, _config);

            for (int index = 0; index < existingUsers.PasswordHash.Length; index++)
            {
                if (passwordHash[index] != existingUsers.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
            }

            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" +
                     loginDto.Email + "'";
            int userId = _dataContext.LoadSingleData<int>(userIdSql);
            return Ok(new Dictionary<string, string>
            {
                {"token", AuthHelper.CreateToken(userId: userId, _config) }
            });
        }
        [AllowAnonymous]
        [HttpPost("LoginComplete")]
        public IActionResult LoginComplete(LoginDto loginDto)
        {
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get 
                @Email = @EmailParam";

            //var sqlParameter = new SqlParameter[]
            //{
            //    new SqlParameter("@EmailParam", loginDto.Email)
            //};

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@EmailParam", loginDto.Email, DbType.String);
            LoginConfirmationDto userForConfirmation = _dataContext
                  .LoadDataSingleWithParameters<LoginConfirmationDto>(sqlForHashAndSalt, dynamicParameters);

            byte[] passwordHash = AuthHelper.GetPasswordHash(loginDto.Password, userForConfirmation.PasswordSalt, _config);

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
            }

            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" +
                     loginDto.Email + "'";
            int userId = _dataContext.LoadSingleData<int>(userIdSql);
            return Ok(new Dictionary<string, string>
            {
                {"token", AuthHelper.CreateToken(userId: userId, _config) }
            });
        }
        [AllowAnonymous]
        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirstValue("userId") + "";
            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                     userId + "'";

            var userIdResult = _dataContext.LoadSingleData<int>(userIdSql);
            var result = new Dictionary<string, string>
            {
                {"token", AuthHelper.CreateToken(userIdResult, _config)},
                {"userId", userId},
                {"First Claim", User.Claims.First().Value ?? null}
            };
            return Ok(result);
        }
        [Authorize]
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(LoginDto loginDto)
        {
            if (AuthHelper.SetPassword(loginDto, _config))
            {
                return Ok();
            }
            return BadRequest();
        }


    }
}
