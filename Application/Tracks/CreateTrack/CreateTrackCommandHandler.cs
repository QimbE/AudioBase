using Application.DataAccess;
using Domain.MusicReleases.Exceptions;
using Domain.Tracks;
using Domain.Tracks.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Application.Tracks.CreateTrack;

public class CreateTrackCommandHandler : IRequestHandler<CreateTrackCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CreateTrackCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(CreateTrackCommand request, CancellationToken cancellationToken)
    {
        // Track name should be unique
        if (await _context.Tracks.AnyAsync(
                r => r.Name.ToLower() == request.Name.ToLower(),
                cancellationToken)
           )
        {
            return new(new TrackWithSameNameException());
        }

        var releaseToFind = await _context.Releases.FirstOrDefaultAsync(r => r.Id == request.ReleaseId);
        if (releaseToFind is null)
        {
            return new(new ReleaseNotFoundException());
        }

        var genreToFind = await _context.Genres.FirstOrDefaultAsync(g => g.Id == request.GenreId);
        if (genreToFind is null)
        {
            return new(new GenreNotFoundException());
        }
        
        /*DateOnly date;
        if (!DateOnly.TryParseExact(request.TrackDate, "yyyy-MM-dd", out date))
        {
            return new(new FormatException());
        }*/
        
        var track = Track.Create(
            request.Name, 
            request.AudioLink, 
            TimeSpan.Parse(request.Duration), 
            request.ReleaseId, 
            request.GenreId);

        _context.Tracks.Add(track);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}