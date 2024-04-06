using Application.DataAccess;
using Domain.MusicReleases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Releases.DeleteRelease;

public class DeleteReleaseCommandHandler : IRequestHandler<DeleteReleaseCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteReleaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteReleaseCommand request, CancellationToken cancellationToken)
    {
        var releaseFromDb = await _context.Releases.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        // if there is no release with given id in DB
        if (releaseFromDb is null)
        {
            return new(new ReleaseNotFoundException(request.Id));
        }

        // Delete release
        _context.Releases.Remove(releaseFromDb);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}