using Application.Authentication;

namespace Infrastructure.Authentication;

public class HashProvider: IHashProvider
{
    public Task<string> HashPassword(string password)
    {
        return Task.FromResult(
            BCrypt.Net.BCrypt.EnhancedHashPassword(password)
            );
    }

    public Task<bool> VerifyPassword(string password, string passwordHash)
    {
        return Task.FromResult(
            BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash)
            );
    }
}