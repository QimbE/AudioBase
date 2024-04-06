using Application.DataAccess;
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

        var release = Release.Create(request.Name, request.CoverLink, request.AuthorId, request.TypeId, request.ReleaseDate);

        _context.Releases.Add(release);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}