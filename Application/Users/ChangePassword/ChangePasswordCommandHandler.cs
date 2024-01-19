using System.Security.Authentication;
using Application.Authentication;
using Application.DataAccess;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.ChangePassword;

public class ChangePasswordCommandHandler: IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashProvider _hashProvider;

    public ChangePasswordCommandHandler(
        IApplicationDbContext context,
        IHashProvider hashProvider
        )
    {
        _context = context;
        _hashProvider = hashProvider;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        // if the id is wrong
        if (userFromDb is null)
        {
            return new(new UserNotFoundException(request.UserId));
        }

        // if the old password is incorrect
        if (!await _hashProvider.VerifyPassword(request.OldPassword, userFromDb.Password))
        {
            return new(new InvalidCredentialException("Invalid credentials"));
        }

        // if the "new" password is the same as the old one
        if (request.OldPassword == request.NewPassword)
        {
            return new(new ChangeToTheSamePasswordException());
        }

        var newPasswordHash = await _hashProvider.HashPassword(request.NewPassword);
        
        userFromDb.Update(
            userFromDb.Name,
            userFromDb.Email,
            newPasswordHash,
            userFromDb.RoleId
            );

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}