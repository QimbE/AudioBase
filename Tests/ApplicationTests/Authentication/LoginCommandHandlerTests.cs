using System.Security.Authentication;
using Application.Authentication;
using Application.Authentication.Login;
using Application.Authentication.Register;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Authentication;

public class LoginCommandHandlerTests: AuthTestingBase
{
    
    public LoginCommandHandlerTests()
        : base(typeof(LoginCommandHandlerTests))
    {
    }
    
    [Theory]
    [InlineData("1@1.ru", "Aasdsdsa123123", "1@1.ru", "bim")]
    [InlineData("1@1.ru", "Aasdsdsa123123", "2@1.ru", "bim")]
    [InlineData("1@1.ru", "Aasdsdsa123123", "2@1.ru", "Aasdsdsa123123")]
    public void Login_Should_ReturnException_OnInvalidCredentials(
        string validEmail,
        string validPassword,
        string actualEmail,
        string actualPassword
        )
    {
        // Arrange
        RecreateDbContext();

        var registerRequest = new RegisterCommand("123", validEmail, validPassword);
        var loginRequest = new LoginCommand(actualEmail, actualPassword);

        var registerHandler = new RegisterCommandHandler(Context, HashProvider);
        var loginHandler = new LoginCommandHandler(Context, HashProvider, JwtProvider);

        registerHandler.Handle(registerRequest, default).GetAwaiter().GetResult();
        
        // Act
        var result = loginHandler.Handle(loginRequest, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match<Exception>(
            success => new Exception(),
            failure => failure
            );

        exception.Should().BeOfType<InvalidCredentialException>();
    }
    
    [Fact]
    public void Login_Should_ReturnUserResponse_OnValidCredentials()
    {
        // Arrange
        RecreateDbContext();

        var email = "1@1.ru";
        var password = "bimbimbim123";
        
        var registerRequest = new RegisterCommand("123", email, password);
        var loginRequest = new LoginCommand(email, password);

        var registerHandler = new RegisterCommandHandler(Context, HashProvider);
        var loginHandler = new LoginCommandHandler(Context, HashProvider, JwtProvider);

        registerHandler.Handle(registerRequest, default).GetAwaiter().GetResult();
        
        var user = Context.Users.FirstOrDefault(u => u.Email == registerRequest.Email);
            
        user!.VerifyEmail();

        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        // Act
        var result = loginHandler.Handle(loginRequest, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var response = result.Match<UserResponse>(
            success => success,
            failure => null
        );

        response.Should().Match<UserResponse>(
            x => !string.IsNullOrWhiteSpace(x.RefreshToken) &&
                 !string.IsNullOrWhiteSpace(x.AccessToken) &&
                 x.Username == registerRequest.Name
        );
    }
    
    [Fact]
    public void Login_Should_ReturnBadRequest_OnUnverifiedEmail()
    {
        // Arrange
        RecreateDbContext();

        var email = "1@1.ru";
        var password = "bimbimbim123";
        
        var registerRequest = new RegisterCommand("123", email, password);
        var loginRequest = new LoginCommand(email, password);

        var registerHandler = new RegisterCommandHandler(Context, HashProvider);
        var loginHandler = new LoginCommandHandler(Context, HashProvider, JwtProvider);

        registerHandler.Handle(registerRequest, default).GetAwaiter().GetResult();
        
        // Act
        var result = loginHandler.Handle(loginRequest, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeFalse();

        var response = result.Match<Exception>(
            success => null,
            failure => failure
        );

        response.Should().BeOfType<UnverifiedEmailException>();
    }
}