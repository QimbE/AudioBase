using Domain.Tracks;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class TrackType: EntityType<Track>
{
    protected override void Configure(IObjectTypeDescriptor<Track> descriptor)
    {
        base.Configure(descriptor);
    }
}