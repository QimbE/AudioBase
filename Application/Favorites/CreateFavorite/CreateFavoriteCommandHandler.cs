using Application.DataAccess;
using Domain.Favorites;
using Domain.Favorites.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Favorites.CreateFavorite;

public class CreateFavoriteCommandHandler: IRequestHandler<CreateFavoriteCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CreateFavoriteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(CreateFavoriteCommand request, CancellationToken cancellationToken)
    {
        // Track should not be favorite already
        if (await _context.Favorites.AnyAsync(
                f => f.UserId == request.UserId 
                     && f.TrackId == request.TrackId,
                cancellationToken)
           )
        {
            return new(new AlreadyFavoriteException());
        }

        // New artist
        var favorite = Favorite.Create(request.UserId, request.TrackId);

        _context.Favorites.Add(favorite);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}