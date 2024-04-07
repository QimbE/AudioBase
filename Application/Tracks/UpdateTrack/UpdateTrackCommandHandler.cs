using Application.DataAccess;
using Domain.MusicReleases.Exceptions;
using Domain.Tracks.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tracks.UpdateTrack;

public class UpdateTrackCommandHandler: IRequestHandler<UpdateTrackCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTrackCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateTrackCommand request, CancellationToken cancellationToken)
    {
        var trackFromDb = await _context.Tracks.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        
        // if there is no track with given id in DB
        if (trackFromDb is null)
        {
            return new(new TrackNotFoundException(request.Id));
        }
        
        // if track with same name is already in DB
        var trackWithSameName = await _context.Tracks.SingleOrDefaultAsync(
            r => r.Name.ToLower() == request.Name.ToLower(),
            cancellationToken);
        if (trackWithSameName is not null && trackWithSameName!=trackFromDb)
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

        // Update track
        trackFromDb.Update(
            request.Name, 
            request.AudioLink, 
            TimeSpan.Parse(request.Duration), 
            request.ReleaseId, 
            request.GenreId);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}