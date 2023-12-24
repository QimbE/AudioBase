namespace Domain.Abstractions;

// TODO: add interfaces and override methods (generic way)
public abstract class Entity
{
    public Guid Id { get; protected set;}

    protected Entity()
    {
        
    }

    public Entity(Guid id)
    {
        Id = id;
    }
}