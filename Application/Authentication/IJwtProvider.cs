using Domain.Users;

namespace Application.Authentication;

public interface IJwtProvider
{
    Task<string> Generate(User user);
    
    string GenerateRefreshToken();
}