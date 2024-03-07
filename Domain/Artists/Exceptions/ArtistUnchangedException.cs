using System.Data;

namespace Domain.Artists.Exceptions;

public class ArtistUnchangedException: DuplicateNameException
{
    public ArtistUnchangedException()
        : base("Artist data is unchanged")
    {
        
    }
}