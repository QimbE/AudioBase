using Domain.Users;
using Infrastructure.Authentication;
using Infrastructure.Email;
using Infrastructure.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;

namespace InfrastructureTests;

public class EmailSenderTests
{
    private readonly JwtSettings _jwtSettings = new JwtSettings
    {
        Issuer = "huh",
        Audience = "bimbimbambam",
        Key = "hehehehuhhehehehehehehehuhhwhwhwhhh",
        ExpiryTime = TimeSpan.FromMinutes(30)
    };
    
    private readonly User _defaultUser = User.Create(
        "Test",
        "test@test.ru",
        "123123",
        Role.List.First()
    );

    private readonly EmailSettings _emailSettings = new EmailSettings
    {
        DisplayName = "Test",
        From = "bimbim@bam.ru",
        Host = "smtp.hehehehuh.ru",
        IsProduction = true,
        Password = "1231231231234",
        Port = 123,
        UserName = "hehehehuh",
        VerificationPageUrl = "https://hehehehuh.ru/verification"
    };

    [Fact]
    public async Task SendVerificationEmail_ShouldNot_SendAnyLetters_InDevelopmentMode()
    {
        // Arrange
        var newEmailSettings = new EmailSettings
        {
            DisplayName = "Test",
            From = "bimbim@bam.ru",
            Host = "smtp.hehehehuh.ru",
            IsProduction = false,
            Password = "1231231231234",
            Port = 123,
            UserName = "hehehehuh",
            VerificationPageUrl = "https://hehehehuh.ru/verification"
        };
        
        var jwtConfigMock = Substitute.For<IOptions<JwtSettings>>();
        jwtConfigMock.Value.Returns(_jwtSettings);

        var provider = new JwtProvider(jwtConfigMock);

        var emailConfigMock = Substitute.For<IOptionsMonitor<EmailSettings>>();
        emailConfigMock.CurrentValue.Returns(newEmailSettings);

        var smtpClientMock = Substitute.For<SmtpClient>();

        var emailService = new EmailSender(emailConfigMock, provider, smtpClientMock);
        
        // Act
        await emailService.SendVerificationEmail(_defaultUser);
        
        // Assert
        smtpClientMock.ReceivedWithAnyArgs(0).Send(new MimeMessage());
    }
    
    [Fact]
    public async Task SendVerificationEmail_Should_SendLetter_InProductionMode()
    {
        // Arrange
        var jwtConfigMock = Substitute.For<IOptions<JwtSettings>>();
        jwtConfigMock.Value.Returns(_jwtSettings);

        var provider = new JwtProvider(jwtConfigMock);

        var emailConfigMock = Substitute.For<IOptionsMonitor<EmailSettings>>();
        emailConfigMock.CurrentValue.Returns(_emailSettings);

        var smtpClientMock = Substitute.For<SmtpClient>();

        smtpClientMock.AuthenticationMechanisms.Returns(["XOAUTH2"]);

        var emailService = new EmailSender(emailConfigMock, provider, smtpClientMock);
        
        // Act
        await emailService.SendVerificationEmail(_defaultUser);
        
        // Assert
        smtpClientMock.ReceivedWithAnyArgs(1).Send(new MimeMessage());
    }
}