using Application.DataAccess;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Logout;

public class LogoutCommandHandler: IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenFromDb = await _context.RefreshTokens.FirstOrDefaultAsync(
            t => t.Value == request.RefreshToken && 
                 t.ExpirationDate > DateTime.UtcNow, cancellationToken
            );

        if (tokenFromDb is null)
        {
            return new(new InvalidRefreshTokenException("Invalid token"));
        }
        
        tokenFromDb.MakeExpire();

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}