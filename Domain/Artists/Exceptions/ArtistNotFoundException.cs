using Domain.Abstractions.Exceptions;

namespace Domain.Artists.Exceptions;

public class ArtistNotFoundException : EntityNotFoundException<Artist>
{
    public ArtistNotFoundException(Guid id)
        : base(id)
    {

    }

    public ArtistNotFoundException()
        : base("There is no such artist in the database")
    {

    }
}