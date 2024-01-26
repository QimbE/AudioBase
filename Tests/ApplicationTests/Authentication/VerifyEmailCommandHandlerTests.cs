using Application.Authentication.VerifyEmail;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;
using Infrastructure.Authentication;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApplicationTests.Authentication;

public class VerifyEmailCommandHandlerTests: AuthTestingBase
{
    private readonly User _defaultUser = User.Create(
        "Test",
        "test@test.ru",
        "123123",
        Role.List.First()
    );
    
    public VerifyEmailCommandHandlerTests()
        : base(typeof(VerifyEmailCommandHandlerTests))
    {
    }

    [Fact]
    public void VerifyEmail_Should_ReturnUnauthorizedAccessException_OnInvalidToken()
    {
        // Arrange
        RecreateDbContext();

        var token = JwtProvider.GenerateVerificationToken(_defaultUser).GetAwaiter().GetResult();
        token += "bim";

        var handler = new VerifyEmailCommandHandler(JwtProvider, Context);
        
        // Act
        var result = handler.Handle(new VerifyEmailCommand(token), default).GetAwaiter().GetResult();
        
        // Assert
        result.IsFaulted.Should().BeTrue();

        var exception = result.Match(
            s => new Exception(),
            f => f
            );

        exception.Should().BeOfType<UnauthorizedAccessException>();
    }
    
    [Fact]
    public void VerifyEmail_Should_ReturnUserNotFoundException_OnValidTokenButNonexistentUser()
    {
        // Arrange
        RecreateDbContext();

        var token = JwtProvider.GenerateVerificationToken(_defaultUser).GetAwaiter().GetResult();

        var handler = new VerifyEmailCommandHandler(JwtProvider, Context);
        
        // Act
        var result = handler.Handle(new VerifyEmailCommand(token), default).GetAwaiter().GetResult();
        
        // Assert
        result.IsFaulted.Should().BeTrue();

        var exception = result.Match(
            s => new Exception(),
            f => f
        );

        exception.Should().BeOfType<UserNotFoundException>();
    }
    
    [Fact]
    public void VerifyEmail_Should_ReturnTrue_OnValidRequest()
    {
        // Arrange
        RecreateDbContext();

        var token = JwtProvider.GenerateVerificationToken(_defaultUser).GetAwaiter().GetResult();

        var handler = new VerifyEmailCommandHandler(JwtProvider, Context);

        Context.Users.Add(_defaultUser);
        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        // Act
        var result = handler.Handle(new VerifyEmailCommand(token), default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var success = result.Match(
            s => true,
            f => false
        );

        success.Should().BeTrue();

        var userFromDb = Context.Users.SingleOrDefault();
        userFromDb.IsVerified.Should().BeTrue();
    }
}