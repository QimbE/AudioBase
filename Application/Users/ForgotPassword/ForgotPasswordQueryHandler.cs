using Application.DataAccess;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.ForgotPassword;

public class ForgotPasswordQueryHandler: IRequestHandler<ForgotPasswordQuery, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    
    public ForgotPasswordQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<bool>> Handle(ForgotPasswordQuery request, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // Nonexistent email
        if (userFromDb is null)
        {
            return new(new UserNotFoundException());
        }
        
        userFromDb.RequestPasswordChange();

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}