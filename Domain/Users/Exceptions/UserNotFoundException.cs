using Domain.Abstractions.Exceptions;

namespace Domain.Users.Exceptions;

public class UserNotFoundException: EntityNotFoundException<User>
{
    public UserNotFoundException(Guid id)
        : base(id)
    {
        
    }

    public UserNotFoundException()
        : base("There is no such user in the database")
    {
        
    }
}