using Application.Authentication;
using Domain.Users;

namespace Infrastructure.Email;

/// <summary>
/// Contains email methods to application users
/// </summary>
public class EmailSender: IEmailSender
{
    // TODO: add actual email sending logic
    public Task SendVerificationEmail(User user)
    {
        user.VerifyEmail();
        return Task.CompletedTask;
    }
}