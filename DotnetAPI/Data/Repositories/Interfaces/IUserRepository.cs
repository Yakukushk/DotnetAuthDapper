using DotnetAPI.Models;

namespace DotnetAPI.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> SaveChanges();
        Task AddEntity<T>(T entityToAdd);
        Task RemoveEntity<T>(T? result);
        Task<IEnumerable<Users>> GetAsync();
        Task<Users> GetSingleAsync(int userId);
        UserJobInfo GetSingleUserJobInfo(int userId);
        UserSalary GetSingleUserSalary(int userId);
    }
}
