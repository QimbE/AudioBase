using Application.DataAccess;
using Domain.Users;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Register;

public class RegisterCommandHandler: IRequestHandler<RegisterCommand, Result<UserResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashProvider _hashProvider;
    private readonly IJwtProvider _jwtProvider;

    public RegisterCommandHandler(IApplicationDbContext context, IHashProvider hashProvider, IJwtProvider jwtProvider)
    {
        _context = context;
        _hashProvider = hashProvider;
        _jwtProvider = jwtProvider;
    }
    
    public async Task<Result<UserResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // if there are the same email
        if (await _context.Users.AnyAsync(
                u=> string.Equals(u.Email, request.Email, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken)
            )
        {
            return new(new UserWithTheSameEmailException());
        }

        var hash = await _hashProvider.HashPassword(request.Password);

        // new user
        var user = User.Create(request.Name, request.Email, hash, Role.DefaultUser);

        _context.Users.Add(user);

        // refreshToken
        var refreshToken = RefreshToken.Create(_jwtProvider.GenerateRefreshToken(), user.Id);

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        var accessToken = await _jwtProvider.Generate(user);

        return new UserResponse(
            user.Id,
            user.Name,
            accessToken,
            refreshToken.Value
            );
    }
}