using DotnetAPI.Application.Caching;
using DotnetAPI.Data;
using DotnetAPI.Data.Repositories;
using DotnetAPI.Data.Repositories.Interfaces;
using DotnetAPI.EndPoints;
using DotnetAPI.Extensions;
using DotnetAPI.Filters;
using DotnetAPI.Infrastructure.Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policies to the service collection
builder.Services.AddCors((options) => // Development
{
    options.AddPolicy("DevCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000") // origin url from front
            .AllowAnyMethod() // could you declare api issues 
            .AllowAnyHeader() // add some kind of weird custom functionality based on weird custom header
            .AllowCredentials(); // token JWT or anothers
    });
    options.AddPolicy("ProdCors", (corsBuilder) => // Production
    {
        corsBuilder.WithOrigins("https://myProductionSite.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddDbContext<DataContextEF>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    string connection = builder.Configuration.GetConnectionString("Redis");
    options.Configuration = connection;

});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();
// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(FilterAction));
    options.Filters.Add(typeof(FilterTimerExecute));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<FilterAction>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());


#region
//SymmetricSecurityKey symmetricSecurityKey =
//           new SymmetricSecurityKey(
//               Encoding.UTF8.GetBytes(
//                   builder.Configuration.GetSection("AppSettings:TokenKey").Value // Retrieving the secret key from configuration
//               ));
//TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
//{
//    IssuerSigningKey = symmetricSecurityKey,
//    ValidateIssuer = false,  // Validation of the issuer mitigates forwarding attacks that can occur when an IdentityProvider represents multiple tenants and signs tokens with the same keys
//    ValidateIssuerSigningKey = false, // It is possible for tokens to contain the public key needed to check the signature
//    ValidateAudience = false // a site that receives a token, could not replay it to another site. 
//};
#endregion
var tokenValidationParameters = SymmetricSecurityKeyExtensions.Instance.TokenValidationParametersExtentions(builder.Configuration.GetSection("AppSettings:TokenKey").Value);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParameters;
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
    app.UseCors("ProdCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();
app.UseRouting();   
RouteGroupBuilder routeGroupBuilder = app.MapGroup("/api");
app.MapEndPoints(routeGroupBuilder);

app.UseAuthentication();
app.UseAuthorization();
app.UseForwardedHeaders();
app.Run();