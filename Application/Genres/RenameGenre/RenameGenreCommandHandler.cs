using Application.DataAccess;
using Domain.Tracks;
using Domain.Tracks.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Genres.RenameGenre;

public class RenameGenreCommandHandler: IRequestHandler<RenameGenreCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RenameGenreCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(RenameGenreCommand request, CancellationToken cancellationToken)
    {
        var genreFromDb = await _context.Genres.SingleOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        
        // if there is no genre with given id in DB
        if (genreFromDb is null)
        {
            return new(new GenreNotFoundException(request.Id));
        }
        
        // if genre with same name is already in DB
        if (await _context.Genres.AnyAsync(
                g => g.Name.ToLower() == request.NewName.ToLower(),
                cancellationToken)
           )
        {
            return new(new GenreWithSameNameException());
        }

        // Update genre
        genreFromDb.Update(request.NewName);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}