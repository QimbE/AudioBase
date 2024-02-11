using Application.DataAccess;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.RequestVerification;

public class RequestVerificationQueryHandler
    : IRequestHandler<RequestVerificationQuery, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RequestVerificationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RequestVerificationQuery request, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (userFromDb is null)
        {
            return new(new UserNotFoundException(request.UserId));
        }
        
        // if this user is verified 
        if (userFromDb.IsVerified)
        {
            return new(new EmailAlreadyVerifiedException());
        }
        
        userFromDb.RequestVerification();

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}