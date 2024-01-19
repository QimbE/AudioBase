using System.Data;

namespace Domain.Users.Exceptions;

public class ChangeToTheSamePasswordException: DuplicateNameException
{
    public ChangeToTheSamePasswordException()
        : base("Your new password must not be the same as previous one")
    {
        
    }
}