using System.Data;

namespace Domain.MusicReleases.Exceptions;

public class ReleaseWithSameNameException: DuplicateNameException
{
    public ReleaseWithSameNameException()
        : base("Release with given name already exists")
    {
        
    }
}