using Domain.Users;

namespace Application.Authentication;

/// <summary>
/// Sends email to user
/// </summary>
public interface IEmailSender
{
    Task SendVerificationEmail(User user);
}