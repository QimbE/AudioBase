using Domain.Abstractions;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public abstract class EntityType<T>
    : ObjectType<T> 
    where T: Entity
{
    protected override void Configure(IObjectTypeDescriptor<T> descriptor)
    {
        base.Configure(descriptor);

        descriptor.Ignore(x => x.DomainEvents);
    }
}