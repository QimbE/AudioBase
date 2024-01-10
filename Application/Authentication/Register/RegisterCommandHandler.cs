using Application.DataAccess;
using Domain.Users;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Register;

/// <summary>
/// Registres new DEFAULT User (Not an admin or smth)
/// </summary>
public class RegisterCommandHandler: IRequestHandler<RegisterCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashProvider _hashProvider;

    public RegisterCommandHandler(IApplicationDbContext context, IHashProvider hashProvider)
    {
        _context = context;
        _hashProvider = hashProvider;
    }
    
    public async Task<Result<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // if there is the same email
        if (await _context.Users.AnyAsync(
                u=> u.Email.ToLower() == request.Email.ToLower(),
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
        var refreshToken = RefreshToken.Create("", user.Id);
        
        // Security reasons
        refreshToken.MakeExpire();

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}