using System.Security.Authentication;
using Application.Authentication.Register;
using Application.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Login;

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
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        
        // if the password is incorrect or the email is invalid
        if (userFromDb is null || 
            ! await _hashProvider.VerifyPassword(request.Password, userFromDb.Password))
        {
            return new(new InvalidCredentialException("Invalid credentials"));
        }
        
        userFromDb.RefreshToken.Update(_jwtProvider.GenerateRefreshToken());

        await _context.SaveChangesAsync(cancellationToken);

        var accessToken = await _jwtProvider.Generate(userFromDb);

        return new UserResponse(
            userFromDb.Id,
            userFromDb.Name,
            accessToken,
            userFromDb.RefreshToken.Value
            );
    }
}