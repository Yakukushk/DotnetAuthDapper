using DotnetAPI.Data;
using DotnetAPI.Models.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAPI.Helpers
{
    public static class AuthHelper
    {
        public static byte[] GetPasswordHash(string password, byte[] passwordSalt, IConfiguration _config)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }

        public static string CreateToken(int userId, IConfiguration _config)
        {
            // Creating an array of claims (key-value pairs) that will be embedded in the token
            Claim[] claims = new Claim[]
            {
        new Claim("userId", userId.ToString()) // Adding the user ID as a claim in the token
            };

            // Creating a symmetric security key using a secret key from the configuration file
            SymmetricSecurityKey symmetricSecurityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _config.GetSection("AppSettings:TokenKey").Value // Retrieving the secret key from configuration
                    ));


            // Generating signing credentials using the security key and HMAC SHA-512 algorithm
            SigningCredentials credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            // Defining the token descriptor, which specifies the token's claims, expiration, and signing credentials
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims), // Attaching the claims (user ID) to the token
                SigningCredentials = credentials, // Assigning signing credentials to ensure token integrity
                Expires = DateTime.Now.AddDays(1), // Setting the token expiration time (valid for 1 day)
            };

            // Creating a JWT security token handler to generate the token
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            // Creating the token based on the descriptor
            SecurityToken token = handler.CreateToken(descriptor);

            // Returning the generated JWT as a string
            return handler.WriteToken(token);
        }
        public static bool SetPassword(LoginDto userForSetPassword, IConfiguration _config)
        {
            var dataContextDapper = new DataContextDapper(_config);


            byte[] passwordSalt = new byte[128 / 8]; // 16 bytes
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt, _config);

            string sqlAddAuth = $@"
                        EXEC TutorialAppSchema.spRegistration_Upsert
                              @Email = '{userForSetPassword.Email}',
                              @PasswordHash = @PasswordSaltComplete,
                              @PasswordSalt = @PasswordHashComplete";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltComplete", SqlDbType.VarBinary);
            passwordSaltParameter.Value = passwordSalt;

            SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashComplete", SqlDbType.VarBinary);
            passwordHashParameter.Value = passwordHash;

            sqlParameters.Add(passwordSaltParameter);
            sqlParameters.Add(passwordHashParameter);
           
            return dataContextDapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
        }

    }
}
