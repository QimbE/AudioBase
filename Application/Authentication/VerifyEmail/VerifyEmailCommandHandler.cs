using Application.DataAccess;
using Domain.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.VerifyEmail;

public class VerifyEmailCommandHandler: IRequestHandler<VerifyEmailCommand, Result<bool>>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IApplicationDbContext _context;

    public VerifyEmailCommandHandler(IJwtProvider jwtProvider, IApplicationDbContext context)
    {
        _jwtProvider = jwtProvider;
        _context = context;
    }

    public async Task<Result<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var email = _jwtProvider.GetEmailFromVerificationToken(request.Token);

        // Token turned out to be invalid or there is no email value
        if (string.IsNullOrWhiteSpace(email))
        {
            return new(new UnauthorizedAccessException("Invalid verification token"));
        }

        var userFromDb = await _context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (userFromDb is null)
        {
            return new(new UserNotFoundException());
        }
        
        userFromDb.VerifyEmail();

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}