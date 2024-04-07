using Application.DataAccess;
using Domain.Tracks.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tracks.DeleteTrack;

public class DeleteTrackCommandHandler : IRequestHandler<DeleteTrackCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteTrackCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteTrackCommand request, CancellationToken cancellationToken)
    {
        var trackFromDb = await _context.Tracks.SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        // if there is no track with given id in DB
        if (trackFromDb is null)
        {
            return new(new TrackNotFoundException(request.Id));
        }

        // Delete track
        _context.Tracks.Remove(trackFromDb);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}