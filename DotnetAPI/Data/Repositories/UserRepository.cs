using DotnetAPI.Data.Repositories.Interfaces;
using DotnetAPI.Models;
using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContextEF _dataContextEF;

        public UserRepository(DataContextEF dataContextEF)
        {
            _dataContextEF = dataContextEF;
        }

        public async Task<bool> SaveChanges()
        {
            return await _dataContextEF.SaveChangesAsync() > 0;
        }

        public async Task AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                await _dataContextEF.AddAsync(entityToAdd);
            }
        }
        public async Task RemoveEntity<T>(T? result)
        {
            if (result != null)
            {
                var newResult = _dataContextEF.Remove(result);
            }

        }

        public async Task<IEnumerable<Users>> GetAsync() =>
            await _dataContextEF.Users.ToListAsync();

        public async Task<Users> GetSingleAsync(int userId)
            => await _dataContextEF.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        public UserSalary GetSingleUserSalary(int userId)
        {
            UserSalary? userSalary = _dataContextEF.UserSalary
                .Where(u => u.UserId == userId)
                .FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Failed to Get User");
        }

        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _dataContextEF.UserJobInfos
                .Where(u => u.UserId == userId)
                .FirstOrDefault<UserJobInfo>();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }

            throw new Exception("Failed to Get User");
        }
    }
}

