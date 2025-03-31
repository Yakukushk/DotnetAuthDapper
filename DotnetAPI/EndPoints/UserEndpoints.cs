using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Data.Repositories.Interfaces;
using DotnetAPI.EndPoints.Interfaces;
using DotnetAPI.Models;
using DotnetAPI.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace DotnetAPI.EndPoints
{
    public class GetAllUsers : IEndPoint
    {
        public void MapUserEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/fetchUser", async (DataContextEF context) =>
            {
                return await context.Users.ToListAsync();
            }).WithOpenApi();

        }
    }
    public class GetUserById : IEndPoint
    {
        public void MapUserEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/fetchUser/{id:int}", async (DataContextEF context, int userId) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
                if (user is null)
                    return Results.NotFound();
                return Results.Ok(user);
            }).WithOpenApi();

        }
    }
    public class PostUser : IEndPoint
    {
        private readonly IMapper _mapper;

        public PostUser(IMapper mapper)
        {
            _mapper = mapper;
        }

        public void MapUserEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("/fetchUser", async (DataContextEF context, [FromBody] UserDTO user) =>
            {
                Users userDb = _mapper.Map<Users>(user);
                if (userDb is null)
                    return Results.BadRequest();
                await context.Users.AddAsync(userDb);
                await context.SaveChangesAsync();
                return Results.Created($"/api/Users/{userDb.UserId}", user);

            }).WithOpenApi();

        }
    }
    public class DeleteUser : IEndPoint
    {
        public void MapUserEndpoints(IEndpointRouteBuilder app)
        {
            app.MapDelete("/fetchUser/{id:int}", async (DataContextEF context, int id) =>
            {
                var userEntity = await context.Users.FirstOrDefaultAsync(x => x.UserId == id);
                if (userEntity is null)
                    return Results.NotFound();
                context.Users.Remove(userEntity);
                await context.SaveChangesAsync();

                return Results.Ok();
            }).WithOpenApi();

        }
    }

    public class UpdateUser : IEndPoint
    {
        public void MapUserEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPut("/fetchUser/{id:int}", async (DataContextEF context, UserDTO user, int id) =>
            {
                var userEntity = await context.Users.FirstOrDefaultAsync(x => x.UserId == id);
                if (userEntity is null)
                    return Results.NotFound();

                user.FirstName = userEntity.FirstName;
                user.LastName = userEntity.LastName;
                user.Email = userEntity.Email;
                user.Gender = userEntity.Gender;
                user.Active = userEntity.Active;

                await context.SaveChangesAsync();
                return Results.Ok();

            }).RequireAuthorization().WithOpenApi();

        }
    }

    public static class EndPointExtension
    {
        public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
        {
            ServiceDescriptor[] serviceDescriptor = assembly
        .DefinedTypes
        .Where(type => typeof(IEndPoint).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
        .Select(x => ServiceDescriptor.Transient(typeof(IEndPoint), x.AsType())) // <-- исправление
        .ToArray();
            foreach (var type in assembly.DefinedTypes)
            {
                if (typeof(IEndPoint).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    Console.WriteLine($"Registered endpoint: {type.FullName}");
                }
            }


            //services.AddTransient<IEndPoint, GetAllUsers>();
            //services.AddTransient<IEndPoint, GetUserById>();
            //services.AddTransient<IEndPoint, PostUser>();
            //services.AddTransient<IEndPoint, DeleteUser>();
            //services.AddTransient<IEndPoint, UpdateUser>();

            services.TryAddEnumerable(serviceDescriptor);
            return services;
        }

        public static IApplicationBuilder MapEndPoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
        {

            IEnumerable<IEndPoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndPoint>>();
            IEndpointRouteBuilder endpointRouteBuilder = routeGroupBuilder is null ? app : routeGroupBuilder;

            foreach (var endpoint in endpoints)
            {
                endpoint.MapUserEndpoints(endpointRouteBuilder);
                Console.WriteLine(endpoint);
            }
            return app;
        }
    }
}
