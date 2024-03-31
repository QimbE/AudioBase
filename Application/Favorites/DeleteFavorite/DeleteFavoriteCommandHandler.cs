using Application.DataAccess;
using Domain.Favorites;
using Domain.Favorites.Exceptions;
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
        RefreshToken? user = _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Value == request.UserToken, cancellationToken).Result;

        if (user is null)
        {
            return new(new UserNotFoundException());
        }

        var fav = _context.Favorites.FirstOrDefaultAsync(
            f => f.UserId == user.Id && f.TrackId == request.TrackId,
            cancellationToken).Result;
        
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