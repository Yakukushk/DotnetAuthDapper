using DotnetAPI.Data;
using DotnetAPI.Data.Repositories;
using DotnetAPI.Data.Repositories.Interfaces;
using DotnetAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddScoped<IUserRepository, UserRepository>();
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
var tokenValidationParameters = SymmetricSecurityKeyExtensions.TokenValidationParametersExtentions(builder.Configuration.GetSection("AppSettings:TokenKey").Value);

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
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();