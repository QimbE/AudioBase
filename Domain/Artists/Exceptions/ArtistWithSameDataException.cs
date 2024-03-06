using System.Data;

namespace Domain.Artists.Exceptions;

public class ArtistWithSameDataException: DuplicateNameException
{
    public ArtistWithSameDataException()
        : base("Author data is unchanged")
    {
        
    }
}