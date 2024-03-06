using System.Data;

namespace Domain.Artists.Exceptions;

public class ArtistWithSameNameException: DuplicateNameException
{
    public ArtistWithSameNameException()
        : base("Author with the same name already exists")
    {
        
    }
}