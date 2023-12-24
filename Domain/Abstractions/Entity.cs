namespace Domain.Abstractions;

public abstract class Entity<TEntity>
    : IEquatable<Entity<TEntity>> 
    where TEntity: Entity<TEntity>
{
    public Guid Id { get; protected set;}

    protected Entity()
    {
        
    }

    public Entity(Guid id)
    {
        Id = id;
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