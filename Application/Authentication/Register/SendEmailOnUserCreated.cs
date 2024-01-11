using Application.DataAccess;
using Domain.Users.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Register;

public class SendEmailOnUserCreated: INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    public SendEmailOnUserCreated(IApplicationDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public async Task Handle(
        UserCreatedDomainEvent notification,
        CancellationToken cancellationToken
        )
    {
        var userFromDb = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);

        // Idk why this could happen, but we have to check if there is such a user
        if (userFromDb is null)
        {
            return;
        }

        // There is no need to send one more verification letter if the user have already verified his/her email
        if (userFromDb.IsVerified)
        {
            return;
        }

        await _emailSender.SendVerificationEmail(userFromDb);
    }
}