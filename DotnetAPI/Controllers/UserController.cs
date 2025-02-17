using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using DotnetAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dataContext;
        private readonly IConfiguration _configuration;
        private readonly ReusableSQL _reusableSql;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _dataContext = new DataContextDapper(_configuration);
            _reusableSql = new ReusableSQL(_configuration);
        }
        [HttpGet("GetUsersAsync")]
        public IEnumerable<Users> GetUsers()
        {
            string sql = @"SELECT * FROM TutorialAppSchema.Users";
            return _dataContext.LoadData<Users>(sql);
        }
        [HttpGet("GetUsersComplete")]
        public IEnumerable<UserComplete> GetUsersComplete(int userId, bool active)
        {
            if (userId != null)
            {
                string sql = $@"EXEC TutorialAppSchema.spUsers_Get @UserId = {userId.ToString()}, @Active = {active.ToString()}";
                return _dataContext.LoadData<UserComplete>(sql);
            }
            throw new Exception("user Id not found");
        }
        [HttpGet("GeSingleUser/{id}")]
        public Users GetSingleUser(int id)
        {
            string sql = $"Select * FROM TutorialAppSchema.Users WHERE UserId = {id}";
            return _dataContext.LoadSingleData<Users>(sql);
        }
        [HttpPut]
        public IActionResult EditUser([FromBody] Users users)
        {

            string firstName = users.FirstName.Replace("'", "''");
            string lastName = users.LastName.Replace("'", "''");
            string email = users.Email.Replace("'", "''");
            string gender = users.Gender.Replace("'", "''");
            int active = users.Active ? 0 : 1;

            string sql = $@"
        UPDATE TutorialAppSchema.Users
        SET [FirstName] = '{firstName}',
            [LastName] = '{lastName}',
            [Email] = '{email}',
            [Gender] = '{gender}',
            [Active] = {active}
        WHERE UserId = {users.UserId}
    ";

            if (_dataContext.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to update user");
        }
        [HttpPost("PostUserComplete")]
        public IActionResult PostUserComplete(UserComplete userComplete)
        {
            #region
            //        string firstName = userComplete.FirstName.Replace("'", "''");
            //        string lastName = userComplete.LastName.Replace("'", "''");
            //        string email = userComplete.Email.Replace("'", "''");
            //        string gender = userComplete.Gender.Replace("'", "''");
            //        string jobTitle = userComplete.JobTitle.Replace("'", "''");
            //        string department = userComplete.Department.Replace("'", "''");
            //        decimal salary = userComplete.Salary;
            //        decimal avgSalary = userComplete.AvgSalary;
            //        int active = userComplete.Active ? 1 : 0;
            //        int userId = userComplete.UserId;

            //        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            //        @FirstName = @FirstNameParameter, 
            //        @LastName = @LastNameParameter, 
            //        @Email = @EmailParameter, 
            //        @Gender = @GenderParameter, 
            //        @Active = @ActiveParameter, 
            //        @JobTitle = @JobTitleParameter, 
            //        @Department = @DepartmentParameter, 
            //        @Salary = @SalaryParameter, 
            //        @UserId = @UserIdParameter";

            //        var parameters = new[]
            //{
            //    new SqlParameter("@FirstName", userComplete.FirstName),
            //    new SqlParameter("@LastName", userComplete.LastName),
            //    new SqlParameter("@Email", userComplete.Email),
            //    new SqlParameter("@Gender", userComplete.Gender),
            //    new SqlParameter("@JobTitle", userComplete.JobTitle),
            //    new SqlParameter("@Department", userComplete.Department),
            //    new SqlParameter("@Salary", userComplete.Salary),
            //    new SqlParameter("@Active", userComplete.Active ? (object)1 : 0), // Use (object) to handle nullable types
            //    new SqlParameter("@User Id", userComplete.UserId)
            //};
            //        if (_dataContext.ExecuteSqlWithParameters(sql, parameters.ToList()))
            //        {
            //            return Ok();
            //        }

            #endregion
            if (_reusableSql.UpsertUser(userComplete))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public IActionResult AddUser([FromBody] UserDTO users)
        {
            string firstName = users.FirstName.Replace("'", "''");
            string lastName = users.LastName.Replace("'", "''");
            string email = users.Email.Replace("'", "''");
            string gender = users.Gender.Replace("'", "''");
            int active = users.Active ? 1 : 0;

            string sql = $@"
        INSERT INTO TutorialAppSchema.Users (
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
        ) VALUES (
            '{firstName}',
            '{lastName}',
            '{email}',
            '{gender}',
            {active}
        )
    ";

            if (_dataContext.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to insert user");
        }
        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] int id)
        {
            string sql = @"
               DELETE FROM TutorialAppSchema.Users 
                 WHERE UserId = " + id.ToString();
            Console.WriteLine(sql);

            if (_dataContext.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete user");
        }
        [HttpDelete("DeleteUserComplete")]
        public IActionResult DeleteUserComplete([FromQuery] int id)
        {
            string sql = $@"EXEC TutorialAppSchema.spUser_Delete @UserId = {id.ToString()}";
            Console.WriteLine(sql);

            if (_dataContext.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete user");
        }
    }
}
