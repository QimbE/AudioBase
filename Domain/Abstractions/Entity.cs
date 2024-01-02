using Throw;

namespace Domain.Abstractions;

/// <summary>
/// Represents some abstract entity with uknown type
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set;}
    
    private readonly List<DomainEvent> _domainEvents = [];

    public List<DomainEvent> DomainEvents => _domainEvents;
    
    protected Entity()
    {
        
    }
    
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Raises new event connected to this entity
    /// </summary>
    /// <param name="domainEvent">Some domain event</param>
    protected void RaiseEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(
            domainEvent.ThrowIfNull()
            );
    }
}

/// <summary>
/// Some entity with specific type.
/// </summary>
/// <typeparam name="TEntity">Actual entity class</typeparam>
public abstract class Entity<TEntity>
    : Entity, IEquatable<Entity<TEntity>> 
    where TEntity: Entity<TEntity>
{

    protected Entity()
    {
        
    }

    protected Entity(Guid id)
        : base(id)
    {
        
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TEntity> other &&
               this.Equals(other);
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"Entity instance of type {typeof(TEntity).Name} with Id: {this.Id}";
    }

    public virtual bool Equals(Entity<TEntity>? other)
    {
        return other.Id == this.Id;
    }
    
    public static bool operator==(Entity<TEntity> first, Entity<TEntity> second)
    {
        return first.Equals(second);
    }
    
    public static bool operator!=(Entity<TEntity> first, Entity<TEntity> second)
    {
        return !(first == second);
    }
}