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

    private const string VerificationMailSubject = "AudioBase Email verification";
    public EmailSender(IOptionsMonitor<EmailSettings> settings, IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
        _settings = settings.CurrentValue;
    }
    
    public async Task SendVerificationEmail(User user)
    {
        if (!_settings.IsProduction)
        {
            return;
        }

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
        email.To.Add(MailboxAddress.Parse(user.Email));
        email.Subject = VerificationMailSubject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = EmailMessageBodies.GetVerificationEmailBody(
                user.Name,
                _settings.VerificationPageUrl, 
                await _jwtProvider.GenerateVerificationToken(user)
                )
        };

        using var client = new SmtpClient();
        client.Connect(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect);
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        client.Authenticate(_settings.UserName, _settings.Password);
        
        client.Send(email);
    }
}

public static class EmailMessageBodies
{
    // TODO: Make cool verification letter (optionally)
    public static string GetVerificationEmailBody(string userName, string verificationEndpointLink, string verificationToken) =>
        $"<p>Dear {userName}!</p> <p>Please verify your email address by clicking here:</p> <a href=\"{verificationEndpointLink}?token={verificationToken}\">link</a>";
}