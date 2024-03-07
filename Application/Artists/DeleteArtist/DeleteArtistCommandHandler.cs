using Application.DataAccess;
using Domain.Artists.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Artists.DeleteArtist;

public class DeleteArtistCommandHandler: IRequestHandler<DeleteArtistCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteArtistCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(DeleteArtistCommand request, CancellationToken cancellationToken)
    {
        var artistFromDb = await _context.Artists.SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        
        // if there is no artist with given id in DB
        if (artistFromDb is null)
        {
            return new(new ArtistNotFoundException(request.Id));
        }
        
        // Delete artist
        _context.Artists.Remove(artistFromDb);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}