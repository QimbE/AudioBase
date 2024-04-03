using Domain.Abstractions.Exceptions;

namespace Domain.Tracks.Exceptions;

public class TrackNotFoundException: EntityNotFoundException<Track>
{
    public TrackNotFoundException(Guid id)
        : base(id)
    {
        
    }

    public TrackNotFoundException()
        : base("There is no such track in the database")
    {
        
    }
}