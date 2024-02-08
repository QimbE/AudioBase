using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Authentication;
using Domain.Users;
using Infrastructure.Options;
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

    public Task<string> GenerateAccessToken(User user)
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

    public Task<string> GenerateVerificationToken(User user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.Email, user.Email)
        };
        
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Value.Key)), 
            SecurityAlgorithms.HmacSha256
            );
        
        var token = new JwtSecurityToken(
            _settings.Value.Issuer,
            _settings.Value.Audience,
            claims,
            null,
            DateTime.UtcNow.Add(
                TimeSpan.FromMinutes(5)
            ),
            signingCredentials
        );

        string tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Task.FromResult(tokenValue);
    }

    public string? GetEmailFromVerificationToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Key));
        
        var validator = new JwtSecurityTokenHandler();
        
        TokenValidationParameters validationParameters = new TokenValidationParameters();
        validationParameters.ValidIssuer = _settings.Value.Issuer;
        validationParameters.ValidAudience = _settings.Value.Audience;
        validationParameters.IssuerSigningKey = key;
        validationParameters.ValidateIssuerSigningKey = true;
        validationParameters.ValidateAudience = true;
        validationParameters.ValidateLifetime = true;

        if (!validator.CanReadToken(token))
        {
            return null;
        }

        ClaimsPrincipal principal;
        try
        {
            // This line throws if invalid
            principal = validator.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
        
        if(principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            return principal.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        }

        return null;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(64)
        );
    }
}