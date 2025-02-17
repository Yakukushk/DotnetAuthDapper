using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Data.Repositories.Interfaces;
using DotnetAPI.Models;
using DotnetAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DotnetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEFController : ControllerBase
    {
        private readonly DataContextEF _dataContextEF;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserEFController(DataContextEF dataContextEF, IUserRepository userRepository)
        {
            _dataContextEF = dataContextEF;
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, Users>()));
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Users>> GetUsers() =>
             await _userRepository.GetAsync();

        [HttpGet("singleUser/{id}")]
        public async Task<Users> GetSingleUser(int id) =>
           await _userRepository.GetSingleAsync(id);

        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] Users user)
        {
            var userEntity =
                await _dataContextEF
                .Users
                .FirstOrDefaultAsync(x => x.UserId == user.UserId);

            if (userEntity != null)
            {
                userEntity.FirstName = user.FirstName;
                userEntity.LastName = user.LastName;
                userEntity.Email = user.Email;
                userEntity.Gender = user.Gender;
                userEntity.Active = user.Active;

                if (await _userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Update User");
            }
            throw new Exception("Failed to Update User");


        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] Users user)
        {
            Users userDb = _mapper.Map<Users>(user);
            var result = _userRepository.AddEntity<Users>(userDb);
            if (result is null)
                return BadRequest("Your form was empty");

            if (await _userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            var result =
                await _dataContextEF
                .Users
                .FirstOrDefaultAsync(x => x.UserId == id);

            await _userRepository.RemoveEntity<Users>(result);

            if (await _userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }
        [HttpGet("UserSalary/{userId}")]
        public UserSalary GetUserSalaryEF(int userId)
        {
            return _userRepository.GetSingleUserSalary(userId);
        }

        [HttpPost("UserSalary")]
        public async Task<IActionResult> PostUserSalaryEf(UserSalary userForInsert)
        {
            await _userRepository.AddEntity<UserSalary>(userForInsert);
            if (await _userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }


        [HttpPut("UserSalary")]
        public async Task<IActionResult> PutUserSalaryEf(UserSalary userForUpdate)
        {
            UserSalary? userToUpdate = _userRepository.GetSingleUserSalary(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (await _userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to Update");
        }


        [HttpDelete("UserSalary/{userId}")]
        public async Task<IActionResult> DeleteUserSalaryEf(int userId)
        {
            UserSalary? userToDelete = _userRepository.GetSingleUserSalary(userId);

            if (userToDelete != null)
            {
                await _userRepository.RemoveEntity<UserSalary>(userToDelete);
                if (await _userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to delete");
        }


        [HttpGet("UserJobInfo/{userId}")]
        public UserJobInfo GetUserJobInfoEF(int userId)
        {
            return _userRepository.GetSingleUserJobInfo(userId);
        }

        [HttpPost("UserJobInfo")]
        public async Task<IActionResult> PostUserJobInfoEf(UserJobInfo userForInsert)
        {
            await _userRepository.AddEntity<UserJobInfo>(userForInsert);
            if (await _userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }


        [HttpPut("UserJobInfo")]
        public async Task<IActionResult> PutUserJobInfoEf(UserJobInfo userForUpdate)
        {
            UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfo(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (await _userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to Update");
        }


        [HttpDelete("UserJobInfo/{userId}")]
        public async Task<IActionResult> DeleteUserJobInfoEf(int userId)
        {
            UserJobInfo? userToDelete = _userRepository.GetSingleUserJobInfo(userId);

            if (userToDelete != null)
            {
                await _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
                if (await _userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }
    }
}
