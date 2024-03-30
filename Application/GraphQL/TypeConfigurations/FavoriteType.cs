using Domain.Favorites;
using HotChocolate.Types;

namespace Application.GraphQL.TypeConfigurations;

public class FavoriteType: EntityType<Favorite>
{
    protected override void Configure(IObjectTypeDescriptor<Favorite> descriptor)
    {
        base.Configure(descriptor);
    }
}