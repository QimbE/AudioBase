using Application.DataAccess;
using Domain.Artists.Exceptions;
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

        var artistToFind = await _context.Artists.FirstOrDefaultAsync(a => a.Id == request.AuthorId);
        if (artistToFind is null)
        {
            return new(new ArtistNotFoundException());
        }

        var typeToFind = await _context.ReleaseTypes.FirstOrDefaultAsync(rt => rt.Value == request.TypeId);
        if (typeToFind is null)
        {
            return new(new ReleaseTypeNotFoundException());
        }

        DateOnly date;
        if (!DateOnly.TryParseExact(request.ReleaseDate, "yyyy-MM-dd", out date))
        {
            return new(new FormatException());
        }

        // Update release
        releaseFromDb.Update(request.Name, request.CoverLink, request.AuthorId, request.TypeId, date);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}