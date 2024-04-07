using Domain.Abstractions.Exceptions;

namespace Domain.MusicReleases.Exceptions;

public class ReleaseNotFoundException : EntityNotFoundException<Release>
{
    public ReleaseNotFoundException(Guid id)
        : base(id)
    {

    }

    public ReleaseNotFoundException()
        : base("There is no such release in the database")
    {

    }
}