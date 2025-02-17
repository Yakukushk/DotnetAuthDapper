using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.SqlServer.Server;

namespace DotnetAPI.Helpers
{
    public class ReusableSQL
    {
        private readonly DataContextDapper _dataContextDapper;
        public ReusableSQL(IConfiguration _config)
        {
            _dataContextDapper = new DataContextDapper(_config);
        }

        public bool UpsertUser(UserComplete userComplete)
        {
            string firstName = userComplete.FirstName.Replace("'", "''");
            string lastName = userComplete.LastName.Replace("'", "''");
            string email = userComplete.Email.Replace("'", "''");
            string gender = userComplete.Gender.Replace("'", "''");
            string jobTitle = userComplete.JobTitle.Replace("'", "''");
            string department = userComplete.Department.Replace("'", "''");
            decimal salary = userComplete.Salary;
            decimal avgSalary = userComplete.AvgSalary;
            int active = userComplete.Active ? 1 : 0;
            int userId = userComplete.UserId;

            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = @FirstNameParameter, 
            @LastName = @LastNameParameter, 
            @Email = @EmailParameter, 
            @Gender = @GenderParameter, 
            @Active = @ActiveParameter, 
            @JobTitle = @JobTitleParameter, 
            @Department = @DepartmentParameter, 
            @Salary = @SalaryParameter, 
            @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParameter", userComplete.FirstName);
            sqlParameters.Add("@LastNameParameter", userComplete.LastName);
            sqlParameters.Add("@EmailParameter", userComplete.Email);
            sqlParameters.Add("@GenderParameter", userComplete.Gender);
            sqlParameters.Add("@JobTitleParameter", userComplete.JobTitle);
            sqlParameters.Add("@DepartmentParameter", userComplete.Department);
            sqlParameters.Add("@SalaryParameter", userComplete.Salary);
            sqlParameters.Add("@ActiveParameter", userComplete.Active ? 1 : 0);
            sqlParameters.Add("@UserIdParameter", userComplete.UserId);

            return _dataContextDapper.ExecuteWithDynamicParameters(sql, sqlParameters);
        }
    }
}
