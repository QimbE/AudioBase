namespace Domain.Users.Exceptions;

public class InvalidRefreshTokenException: ArgumentException
{
    public InvalidRefreshTokenException(string message)
        : base(message)
    {
        
    }
}