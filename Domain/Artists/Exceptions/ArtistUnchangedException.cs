using System.Data;

namespace Domain.Artists.Exceptions;

public class ArtistUnchangedException: ArgumentException
{
    public ArtistUnchangedException()
        : base("Artist data is unchanged")
    {
        
    }
}