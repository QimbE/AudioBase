using Application.DataAccess;
using Domain.MusicReleases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Releases.UpdateRelease;

public class UpdateReleaseCommandHandler: IRequestHandler<UpdateReleaseCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateReleaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateReleaseCommand request, CancellationToken cancellationToken)
    {
        var releaseFromDb = await _context.Releases.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        
        // if there is no release with given id in DB
        if (releaseFromDb is null)
        {
            return new(new ReleaseNotFoundException(request.Id));
        }
        
        // if release with same name is already in DB
        var releaseWithSameName = await _context.Releases.SingleOrDefaultAsync(
            r => r.Name.ToLower() == request.Name.ToLower(),
            cancellationToken);
        if (releaseWithSameName is not null && releaseWithSameName!=releaseFromDb)
        {
            return new(new ReleaseWithSameNameException());
        }

        // Update release
        releaseFromDb.Update(request.Name, request.CoverLink, request.AuthorId, request.TypeId, request.ReleaseDate);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}