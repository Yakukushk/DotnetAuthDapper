using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Buffers;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dataContext;
        public PostController(IConfiguration _config)
        {
            _dataContext = new DataContextDapper(_config);
        }

        [HttpGet("Posts")]
        public IEnumerable<Posts> GetPosts()
        {
            string sql = @"SELECT * FROM TutorialAppSchema.Posts";
            return _dataContext.LoadData<Posts>(sql);
        }
        [HttpGet("PostsBySearch/{searchParams}")]
        public IEnumerable<Posts> GetPostsBySearch(string searchParams)
        {
            string sql = @"SELECT * FROM TutorialAppSchema.Posts
     WHERE PostTitle LIKE '%" + searchParams + "%'" +
     " OR PostContent LIKE '%" + searchParams + "%'";
            return _dataContext.LoadData<Posts>(sql);
        }

        [HttpGet("GetPostComplete")]
        public IEnumerable<Posts> GetPostComplete([FromQuery] int userId, string searchValue, int postId)
        {
            string sql = $@"EXEC TutorialAppSchema.spPosts_Get @UserId = {userId.ToString()}, @SearchValue = '{searchValue}', @PostId = {postId.ToString()}";
            return _dataContext.LoadData<Posts>(sql);
        }

        [HttpGet("Post/{id}")]
        public Posts GetPost(int id)
        {
            string sql = @"SELECT * FROM TutorialAppSchema.Posts WHERE PostId = " + id.ToString();
            return _dataContext.LoadSingleData<Posts>(sql);
        }
        [HttpGet("PostByUser")]
        public IEnumerable<Posts> GetPostsByUser()
        {
            string sql = @"SELECT * FROM TutorialAppSchema.Posts WHERE UserId = " + this.User.FindFirst("userId")?.Value;
            return _dataContext.LoadData<Posts>(sql);
        }
        [HttpGet("PostByUserComplete")]
        public IEnumerable<Posts> GetPostsByUserComplete()
        {
            string sql = $@"EXEC TutorialAppSchema.spPosts_Get @UserId = {User.FindFirst("userId")?.Value}";
            return _dataContext.LoadData<Posts>(sql);
        }
        [HttpPost("AddPost")]
        public IActionResult AddUser([FromBody] PostDto post)
        {
            string userId = this.User.FindFirst("userId")?.Value.Replace("'", "''");
            string postTitle = post.PostTitle.Replace("'", "''");
            string postContent = post.PostContent.Replace("'", "''");

            string sql = @"
        INSERT INTO TutorialAppSchema.Posts (
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated]
        ) VALUES (
            @UserId,
            @PostTitle,
            @PostContent,
            GETDATE(),
            GETDATE()
        )";
            var parameters = new[]
{
        new SqlParameter("@UserId", userId),
        new SqlParameter("@PostTitle", post.PostTitle),
        new SqlParameter("@PostContent", post.PostContent)
    };
            if (_dataContext.ExecuteSqlWithParameters(sql, parameters.ToList()))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post");
        }
        [HttpPut("AddPost")]
        public IActionResult EditUser([FromBody] PostEditDto post)
        {
            string userId = this.User.FindFirst("userId")?.Value.Replace("'", "''");
            string postTitle = post.PostTitle.Replace("'", "''");
            string postContent = post.PostContent.Replace("'", "''");

            string sql = $@"
        UPDATE TutorialAppSchema.Users
        SET [PostContent] = '{postContent}',
            [PostTitle] = '{postTitle}',
            [PostUpdated] = 'GetDate()',
        WHERE PostId = {post.PostId}
    ";

            var parameters = new[]
{
        new SqlParameter("@UserId", userId),
        new SqlParameter("@PostTitle", post.PostTitle),
        new SqlParameter("@PostContent", post.PostContent)
    };
            if (_dataContext.ExecuteSqlWithParameters(sql, parameters.ToList()))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post");
        }
        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.Posts WHERE PostId = " + postId.ToString();
            if (_dataContext.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete new post");
        }

    }
}
