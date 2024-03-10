using Application.DataAccess;
using Domain.Artists.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Artists.UpdateArtist;

public class UpdateArtistCommandHandler: IRequestHandler<UpdateArtistCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateArtistCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateArtistCommand request, CancellationToken cancellationToken)
    {
        var artistFromDb = await _context.Artists.SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        
        // if there is no artist with given id in DB
        if (artistFromDb is null)
        {
            return new(new ArtistNotFoundException(request.Id));
        }
        
        // if artist with same name is already in DB
        var artistWithSameName = await _context.Artists.SingleOrDefaultAsync(
            a => a.Name.ToLower() == request.Name.ToLower(),
            cancellationToken);
        if (artistWithSameName is not null && artistWithSameName!=artistFromDb)
        {
            return new(new ArtistWithSameNameException());
        }

        // Update artist
        artistFromDb.Update(request.Name, request.Description, request.PhotoLink);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}