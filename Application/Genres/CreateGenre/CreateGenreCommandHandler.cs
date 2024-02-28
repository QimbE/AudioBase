using Application.DataAccess;
using Domain.Tracks;
using Domain.Tracks.Exceptions;
using Domain.Users;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Genres.CreateGenre;

public class CreateGenreCommandHandler: IRequestHandler<CreateGenreCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CreateGenreCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        // if genre with same name is already in DB
        if (await _context.Genres.AnyAsync(
                g => g.Name.ToLower() == request.Name.ToLower(),
                cancellationToken)
           )
        {
            return new(new GenreWithSameNameException());
        }

        // New genre
        var genre = Genre.Create(request.Name);

        _context.Genres.Add(genre);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}