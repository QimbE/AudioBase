using Domain.Users;

namespace Application.Authentication;

public interface IJwtProvider
{
    Task<string> GenerateAccessToken(User user);

    Task<string> GenerateVerificationToken(User user);
    
    string GenerateRefreshToken();

    public string? GetEmailFromVerificationToken(string token);
}