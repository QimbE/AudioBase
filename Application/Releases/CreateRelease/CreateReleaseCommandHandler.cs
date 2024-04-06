using Application.DataAccess;
using Domain.Artists.Exceptions;
using Domain.MusicReleases;
using Domain.MusicReleases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Releases.CreateRelease;

public class CreateReleaseCommandHandler : IRequestHandler<CreateReleaseCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CreateReleaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(CreateReleaseCommand request, CancellationToken cancellationToken)
    {
        // Release name should be unique
        if (await _context.Releases.AnyAsync(
                r => r.Name.ToLower() == request.Name.ToLower(),
                cancellationToken)
           )
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

        var release = Release.Create(request.Name, request.CoverLink, request.AuthorId, request.TypeId, date);

        _context.Releases.Add(release);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}