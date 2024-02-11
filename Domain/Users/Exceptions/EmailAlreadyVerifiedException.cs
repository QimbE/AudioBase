namespace Domain.Users.Exceptions;

public class EmailAlreadyVerifiedException: ArgumentException
{
    public EmailAlreadyVerifiedException()
        : base("This email address has already been verified")
    {
        
    }
}