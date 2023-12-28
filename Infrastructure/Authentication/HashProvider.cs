using Application.Authentication;

namespace Infrastructure.Authentication;

public class HashProvider: IHashProvider
{
    public Task<string> HashPassword(string password)
    {
        return Task.FromResult(
            BCrypt.Net.BCrypt.HashPassword(password)
            );
    }
}