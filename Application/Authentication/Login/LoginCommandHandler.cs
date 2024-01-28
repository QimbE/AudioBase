using System.Security.Authentication;
using Application.Authentication.Register;
using Application.DataAccess;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Login;

/// <summary>
/// Logs user in.
/// </summary>
/// <example>
/// Base use cases:
/// When user wants to have an access with new device/browser.
/// When refresh token has expired and user have to login again.
/// </example>
public class LoginCommandHandler: IRequestHandler<LoginCommand, Result<UserResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashProvider _hashProvider;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IApplicationDbContext context, IHashProvider hashProvider, IJwtProvider jwtProvider)
    {
        _context = context;
        _hashProvider = hashProvider;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<UserResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.RefreshToken)
            .SingleOrDefaultAsync(
                u => u.Email.ToLower() == request.Email.ToLower(),
                cancellationToken
                );
        
        // if the password is incorrect or the email is invalid
        if (userFromDb is null || 
            ! await _hashProvider.VerifyPassword(request.Password, userFromDb.Password))
        {
            return new(new InvalidCredentialException("Invalid credentials"));
        }
        
        // if the user is not verified
        if (!userFromDb.IsVerified)
        {
            userFromDb.RequestVerification();
            await _context.SaveChangesAsync(cancellationToken);
            return new(new UnverifiedEmailException());
        }
        
        userFromDb.RefreshToken.Update(_jwtProvider.GenerateRefreshToken());

        await _context.SaveChangesAsync(cancellationToken);

        var accessToken = await _jwtProvider.GenerateAccessToken(userFromDb);

        return new UserResponse(
            userFromDb.Id,
            userFromDb.Name,
            accessToken,
            userFromDb.RefreshToken.Value
            );
    }
}