namespace Application.Authentication;

public interface IHashProvider
{
    Task<string> HashPassword(string password);
}