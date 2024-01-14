using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Authentication;
using Domain.Users;
using Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class JwtProvider: IJwtProvider
{
    private readonly IOptions<JwtSettings> _settings;

    public JwtProvider(IOptions<JwtSettings> settings)
    {
        _settings = settings;
    }

    public Task<string> Generate(User user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, Role.FromValue(user.RoleId).Name),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };
        
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Value.Key)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _settings.Value.Issuer,
            _settings.Value.Audience,
            claims,
            null,
            DateTime.UtcNow.Add(
                _settings.Value.ExpiryTime
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