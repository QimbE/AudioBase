using Domain.Abstractions.Exceptions;

namespace Domain.Users.Exceptions;

public class UserNotFoundException: EntityNotFoundException<User>
{
    public UserNotFoundException(Guid id)
        : base(id)
    {
        
    }
}