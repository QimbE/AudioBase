using Application.Authentication;
using Domain.Users;
using Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Email;

/// <summary>
/// Contains email methods to application users
/// </summary>
public class EmailSender: IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly IJwtProvider _jwtProvider;
    private readonly SmtpClient _client;

    public EmailSender(IOptionsMonitor<EmailSettings> settings, IJwtProvider jwtProvider, SmtpClient client)
    {
        _jwtProvider = jwtProvider;
        _client = client;
        _settings = settings.CurrentValue;
    }
    
    public async Task SendVerificationEmail(User user)
    {
        user.VerifyEmail();
        // var (subject, body) = EmailMessageBodies.GetVerificationEmailContent(
        //     user.Name,
        //     _settings.VerificationPageUrl,
        //     await _jwtProvider.GenerateVerificationToken(user)
        //     );
        // var email = GetMessage(
        //     _settings.DisplayName,
        //     _settings.From,
        //     user.Email,
        //     subject,
        //     body
        //     );
        // SendMessage(email);
    }

    public async Task SendChangePasswordEmail(User user, string newPassword)
    {
        var (subject, body) = EmailMessageBodies.GetChangePasswordEmailContent(
            user.Name,
            newPassword
        );
        var email = GetMessage(
            _settings.DisplayName,
            _settings.From,
            user.Email,
            subject,
            body
        );
        SendMessage(email);
    }

    private void SendMessage(MimeMessage email)
    {
        if (!_settings.IsProduction)
        {
            return;
        }
        
        _client.Connect(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect);
        _client.AuthenticationMechanisms.Remove("XOAUTH2");
        _client.Authenticate(_settings.UserName, _settings.Password);
        
        _client.Send(email);
    }

    protected static MimeMessage GetMessage(string displayName, string from, string addressee, string subject, string messageBody)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(displayName, from));
        email.To.Add(MailboxAddress.Parse(addressee));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = messageBody
        };

        return email;
    }
}

public static class EmailMessageBodies
{
    // TODO: Make cool letters (optionally)
    public static (string, string) GetVerificationEmailContent(string userName, string verificationEndpointLink, string verificationToken) =>
        ("AudioBase Email verification", $"<p>Dear {userName}!</p> <p>Please verify your email address by clicking here:</p> <a href=\"{verificationEndpointLink}?token={verificationToken}\">link</a>");
    
    public static (string, string) GetChangePasswordEmailContent(string userName, string newPassword) =>
        ("AudioBase Email verification", $"<p>Dear {userName}!</p> <p>We have received a request to change password for the account with this email. Your new password is {newPassword}</p> ");
}