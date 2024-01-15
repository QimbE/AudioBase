using Application.DataAccess;
using Domain.Users;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.ChangeRole;

public class ChangeRoleCommandHandler: IRequestHandler<ChangeRoleCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    
    public ChangeRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(ChangeRoleCommand request, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users.SingleOrDefaultAsync(
            u => u.Id == request.UserId, cancellationToken);

        if (userFromDb is null)
        {
            return new(new UserNotFoundException(request.UserId));
        }
        
        userFromDb.Update(
            userFromDb.Name,
            userFromDb.Email,
            userFromDb.Password,
            Role.FromName(request.RoleName, true)
            );

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}