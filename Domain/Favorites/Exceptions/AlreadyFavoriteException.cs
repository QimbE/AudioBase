using System.Data;
using System.Data.Common;

namespace Domain.Favorites.Exceptions;

public class AlreadyFavoriteException: DuplicateNameException
{
    public AlreadyFavoriteException()
        : base("Track is already favorite")
    {
        
    }
}