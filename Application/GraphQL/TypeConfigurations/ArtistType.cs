using Domain.Artists;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class ArtistType: EntityType<Artist>
{
    protected override void Configure(IObjectTypeDescriptor<Artist> descriptor)
    {
        base.Configure(descriptor);
    }
}