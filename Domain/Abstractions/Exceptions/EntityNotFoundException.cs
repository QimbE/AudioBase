namespace Domain.Abstractions.Exceptions;

public abstract class NotFoundException: Exception
{
    public NotFoundException(string message)
        : base(message)
    {
        
    }
}

public abstract class EntityNotFoundException<T> : NotFoundException
    where T: Entity
{
    public EntityNotFoundException(Guid id)
        : base($"Entity of type {typeof(T)} with Id {id} was not found in database")
    {
        
    }
    
    public EntityNotFoundException(string message)
        : base(message)
    {
        
    }
}