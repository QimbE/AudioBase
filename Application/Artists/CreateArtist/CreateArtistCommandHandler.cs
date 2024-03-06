
using Application.DataAccess;
using Domain.Artists;
using Domain.Artists.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Artists.CreateArtist;

public class CreateArtistCommandHandler : IRequestHandler<CreateArtistCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CreateArtistCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(CreateArtistCommand request, CancellationToken cancellationToken)
    {
        // Artist name should be unique
        if (await _context.Artists.AnyAsync(
                g => g.Name.ToLower() == request.Name.ToLower(),
                cancellationToken)
           )
        {
            return new(new ArtistWithSameDataException());
        }

        // New artist
        var artist = Artist.Create(request.Name, request.Description, request.PhotoLink);

        _context.Artists.Add(artist);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}