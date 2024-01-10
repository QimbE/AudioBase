namespace Domain.Users.Exceptions;

public class UnverifiedEmailException: Exception
{
    public UnverifiedEmailException()
        : base("You must have verified Email address to login to your account")
    {
        
    }
}