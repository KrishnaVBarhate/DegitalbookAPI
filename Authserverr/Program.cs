using Authserverr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddCors((setup) =>
{
    setup.AddPolicy("default", (options) =>
    {
        options.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("default");
app.MapPost("/ValidateLogin", object (UserValidation request, HttpContext http, ITokenService tokenService) =>
{
    var user = request.ValidateCredentials(request.userName, request.password);
    if(user !=null )
    {
        var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"],
                                            builder.Configuration["Jwt:Issuer"],
                                            new[]
                                            {
                                                        builder.Configuration["Jwt:Aud1"]
                                                       
                                                    },
                                            request.userName);
        return new
        {
            Token = token,
            IsAuthenticated = true,
            User= user

        };
    }
    return new
    {
        Token = string.Empty,
        IsAuthenticated = false
    };
})
.WithName("Validate");


await app.RunAsync();


internal record UserValidationRequestModel([Required] string UserName, [Required] string Password);

internal interface ITokenService
{
    string BuildToken(string key, string issuer, IEnumerable<string> audience, string userName);
}
internal class TokenService : ITokenService
{
    private TimeSpan ExpiryDuration = new TimeSpan(20, 30, 0);
    public string BuildToken(string key, string issuer, IEnumerable<string> audience, string userName)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
        };

        claims.AddRange(audience.Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
            expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

}
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
