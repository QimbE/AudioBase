using Application.DataAccess;
using Domain.Favorites;
using Domain.Favorites.Exceptions;
using Domain.Tracks;
using Domain.Tracks.Exceptions;
using Domain.Users;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Favorites.CreateFavorite;

public class AddFavoriteCommandHandler: IRequestHandler<AddFavoriteCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public AddFavoriteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user is null)
        {
            return new(new UserNotFoundException());
        }
        
        Track? track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == request.TrackId);

        if (track is null)
        {
            return new(new TrackNotFoundException());
        }
        
        // Track should not be favorite already
        if (await _context.Favorites.AnyAsync(
                f => f.UserId == request.UserId
                     && f.TrackId == request.TrackId,
                cancellationToken)
           )
        {
            return new(new AlreadyFavoriteException());
        }

        // New favorite
        var favorite = Favorite.Create(user.Id, request.TrackId);

        _context.Favorites.Add(favorite);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}