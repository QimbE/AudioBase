using System.Data;

namespace Domain.Users.Exceptions;

public class UserWithTheSameEmailException: DuplicateNameException
{
    public UserWithTheSameEmailException()
        :base("User with such Email already registered.")
    {
        
    }
}