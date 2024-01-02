using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Authentication;
using Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class JwtProvider: IJwtProvider
{
    private readonly IConfiguration _configuration;

    public JwtProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public Task<string> Generate(User user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, Role.FromValue(user.RoleId).Name),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };
        
        var settings = _configuration.GetSection("JwtSettings");
        
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(settings["Key"])),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            settings["Issuer"],
            settings["Audience"],
            claims,
            null,
            DateTime.UtcNow.Add(
                TimeSpan.Parse(
                    settings["ExpiryTime"]!
                )
            ),
            signingCredentials
        );

        string tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Task.FromResult(tokenValue);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64)
        );
    }
}