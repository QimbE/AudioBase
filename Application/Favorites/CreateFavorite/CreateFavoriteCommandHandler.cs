using Application.DataAccess;
using Domain.Favorites;
using Domain.Favorites.Exceptions;
using Domain.Users;
using Domain.Users.Exceptions;
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
        User? user = _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId).Result;

        if (user is null)
        {
            return new(new UserNotFoundException());
        }
        
        // Track should not be favorite already
        if (await _context.Favorites.AnyAsync(
                f => f.UserId == user.Id
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