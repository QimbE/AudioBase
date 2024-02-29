using Domain.Abstractions.Exceptions;

namespace Domain.Tracks.Exceptions;

public class GenreNotFoundException : EntityNotFoundException<Genre>
{
    public GenreNotFoundException(Guid id)
        : base(id)
    {

    }

    public GenreNotFoundException()
        : base("There is no such genre in the database")
    {

    }
}