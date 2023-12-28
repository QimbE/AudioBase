namespace Application.Authentication;

public interface IHashProvider
{
    Task<string> HashPassword(string password);

    Task<bool> VerifyPassword(string password, string passwordHash);
}