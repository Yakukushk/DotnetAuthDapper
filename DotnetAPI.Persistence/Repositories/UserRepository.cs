using DotnetAPI.Data.Repositories.Interfaces;
using DotnetAPI.Models;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Application.Caching;
using DotnetAPI.Models.DTOs;

namespace DotnetAPI.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContextEF _dataContextEF;
        private readonly ICacheService _cacheService;

        public UserRepository(DataContextEF dataContextEF, ICacheService cacheService)
        {
            _dataContextEF = dataContextEF;
            _cacheService = cacheService;

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
            await _cacheService.RemoveAsync("users");
        }

        public async Task<IEnumerable<object>> GetAsync()
        {
            var usersResponses = await _cacheService.GetAsync<List<UserDTO>>("users");

            if (usersResponses is not null)
            {
                return usersResponses;
            }

            List<UserDTO> users = await _dataContextEF.Users
           .Select(x => new UserDTO
           {
               FirstName = x.FirstName,
               LastName = x.LastName,
               Email = x.Email,
               Gender = x.Gender ?? "undefined",
               Active = x.Active
           })
           .ToListAsync();

            usersResponses = users.ToList();

            await _cacheService.SetAsync("users", usersResponses);
            return usersResponses;

        }


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

