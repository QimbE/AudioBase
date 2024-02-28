using Domain.Tracks;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class GenreType: EntityType<Genre>
{
    protected override void Configure(IObjectTypeDescriptor<Genre> descriptor)
    {
        base.Configure(descriptor);
    }
}