using System.Data;

namespace Domain.Tracks.Exceptions;

public class TrackWithSameNameException: DuplicateNameException
{
    public TrackWithSameNameException()
        : base("Track with the same name already exists")
    {
        
    }
}