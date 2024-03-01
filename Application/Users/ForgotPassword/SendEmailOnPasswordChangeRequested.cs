using System.Security.Cryptography;
using Application.Authentication;
using Application.DataAccess;
using Domain.Users.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.ForgotPassword;

public class SendEmailOnPasswordChangeRequested: INotificationHandler<PasswordChangeRequestedDomainEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IHashProvider _hashProvider;

    public SendEmailOnPasswordChangeRequested(IApplicationDbContext context, IEmailSender emailSender, IHashProvider hashProvider)
    {
        _context = context;
        _emailSender = emailSender;
        _hashProvider = hashProvider;
    }

    public async Task Handle(PasswordChangeRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);

        // Idk why this could happen, but we have to check if there is such a user
        if (userFromDb is null)
        {
            return;
        }

        var newPassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(20));
        
        userFromDb.Update(
            userFromDb.Name,
            userFromDb.Email,
            await _hashProvider.HashPassword(newPassword),
            userFromDb.RoleId);
        
        await _emailSender.SendChangePasswordEmail(userFromDb, newPassword);
    }
}