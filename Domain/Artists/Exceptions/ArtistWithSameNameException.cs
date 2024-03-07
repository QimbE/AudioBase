using System.Data;

namespace Domain.Artists.Exceptions;

public class ArtistWithSameNameException: DuplicateNameException
{
    public ArtistWithSameNameException()
        : base("Artist with given name already exists")
    {
        
    }
}