using Domain.Abstractions.Exceptions;

namespace Domain.Favorites.Exceptions;

public class FavoriteNotFoundException : EntityNotFoundException<Favorite>
{
    public FavoriteNotFoundException(Guid id)
        : base(id)
    {

    }

    public FavoriteNotFoundException()
        : base("Track is already not favorite")
    {

    }
}