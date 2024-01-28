using Application.DataAccess;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Refresh;

/// <summary>
/// Provides new access token if refresh token is still valid
/// </summary>
public class RefreshCommandHandler: IRequestHandler<RefreshCommand, Result<TokenResponse>>
{
    private readonly IApplicationDbContext _context;

    private readonly IJwtProvider _jwtProvider;

    public RefreshCommandHandler(IApplicationDbContext context, IJwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync(
                u => u.RefreshToken.Value == request.RefreshToken &&
                     u.RefreshToken.ExpirationDate > DateTime.UtcNow,
                cancellationToken);

        // If the token is expired or invalid
        if (userFromDb is null)
        {
            return new(new InvalidRefreshTokenException("Invalid token"));
        }

        return new TokenResponse(await _jwtProvider.GenerateAccessToken(userFromDb));
    }
}