using System.Data;

namespace Domain.Favorites.Exceptions;

public class AlreadyFavoriteException: DataException
{
    public AlreadyFavoriteException()
        : base("Track is already favorite")
    {
        
    }
}