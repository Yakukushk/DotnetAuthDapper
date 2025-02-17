using Dapper;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DotnetAPI.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _connection;

        public DataContextDapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public IEnumerable<T> LoadData<T>(string path)
        {
            return _connection.Query<T>(path);
        }

        public T LoadSingleData<T>(string path)
        {
            return _connection.QuerySingle<T>(path);
        }
        public bool ExecuteSql(string path)
        {
            return _connection.Execute(path) > 0;
        }
        public int ExecuteSqlWithRowCount(string path)
        {
            return _connection.Execute(path);
        }
        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            SqlCommand commandWithParams = new SqlCommand(sql);

            foreach (SqlParameter parameter in parameters)
            {
                commandWithParams.Parameters.Add(parameter);
            }

            SqlConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            dbConnection.Open();

            commandWithParams.Connection = dbConnection;

            int rowsAffected = commandWithParams.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;
        }
        public bool ExecuteWithDynamicParameters(string sql, DynamicParameters dynamicParameters)
        {
            return _connection.Execute(sql, dynamicParameters) > 0;
        }
        public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql, parameters);
        }
    }
}
