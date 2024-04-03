using Application.DataAccess;
using Domain.Favorites;
using Domain.Favorites.Exceptions;
using Domain.Tracks;
using Domain.Tracks.Exceptions;
using Domain.Users;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Favorites.DeleteFavorite;

public class DeleteFavoriteCommandHandler: IRequestHandler<DeleteFavoriteCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteFavoriteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(DeleteFavoriteCommand request, CancellationToken cancellationToken)
    {
        // There is no need to check whether user or track exists
        var fav = await _context.Favorites.FirstOrDefaultAsync(
            f => f.UserId == request.UserId && f.TrackId == request.TrackId,
            cancellationToken);
        
        // Track should not be favorite already
        if (fav is null)
        {
            return new(new FavoriteNotFoundException());
        }
        
        _context.Favorites.Remove(fav);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}