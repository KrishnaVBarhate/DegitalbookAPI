using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System.Text;



var builder = WebApplication.CreateBuilder(args);



// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot().AddPolly();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();



app.UseOcelot().Wait();



app.MapControllers();



app.UseAuthentication();
app.UseAuthorization();



app.Run();








//Wrong Code
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.IdentityModel.Tokens;
//using Ocelot.DependencyInjection;
//using Ocelot.Middleware;
//using Ocelot.Provider.Polly;
//using System.ComponentModel.DataAnnotations;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//Add services to the container.

//builder.Services.AddControllers();
//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton<ITokenService>(new TokenService());
//builder.Configuration.AddJsonFile("ocelot.json");
//builder.Services.AddOcelot().AddPolly();
//var app = builder.Build();

//Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.MapPost("/validate", [AllowAnonymous] (UserValidationRequestModel request, HttpContext http, ITokenService tokenService) =>
//{
//    if (request is UserValidationRequestModel { UserName: "krishna", Password: "pass" })
//    {
//        var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"],
//                                            builder.Configuration["Jwt:Issuer"],
//                                            new[]
//                                            {
//                                                        builder.Configuration["Jwt:Aud1"],
//                                                        builder.Configuration["Jwt:Aud2"]
//                                                    },
//                                            request.UserName);
//        return new
//        {
//            Token = token,
//            IsAuthenticated = true,
//        };
//    }
//    return new
//    {
//        Token = string.Empty,
//        IsAuthenticated = false
//    };
//})
//.WithName("Validate");


//await app.RunAsync();
//app.UseOcelot().Wait();

//internal record UserValidationRequestModel([Required] string UserName, [Required] string Password);

//internal interface ITokenService
//{
//    string BuildToken(string key, string issuer, IEnumerable<string> audience, string userName);
//}
//internal class TokenService : ITokenService
//{
//    private TimeSpan ExpiryDuration = new TimeSpan(20, 30, 0);
//    public string BuildToken(string key, string issuer, IEnumerable<string> audience, string userName)
//    {
//        var claims = new List<Claim>
//        {
//            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
//        };

//        claims.AddRange(audience.Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));

//        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
//        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
//        var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
//            expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);
//        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
//    }

//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
