using System.Data;

namespace Domain.Tracks.Exceptions;

public class GenreWithSameNameException: DuplicateNameException
{
    public GenreWithSameNameException()
        : base("Genre with the same name already exists")
    {
        
    }
}