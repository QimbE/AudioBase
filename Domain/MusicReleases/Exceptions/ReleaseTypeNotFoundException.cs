using Domain.Abstractions.Exceptions;

namespace Domain.MusicReleases.Exceptions;

public class ReleaseTypeNotFoundException: NotFoundException
{
    public ReleaseTypeNotFoundException()
        : base("There is no such release type in the database")
    {

    }
}