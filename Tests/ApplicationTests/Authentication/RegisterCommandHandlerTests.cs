using Application.Authentication;
using Application.Authentication.Register;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Authentication;

public class RegisterCommandHandlerTests: AuthTestingBase
{
    public RegisterCommandHandlerTests()
        : base(typeof(RegisterCommandHandlerTests))
    {
        
    }
    
    [Theory]
    [InlineData("email@a.ru", "EmAiL@a.rU")]
    [InlineData("BobaN@ab.ru", "boban@ab.ru")]
    public void Register_Should_ReturnException_OnDuplicateEmail(string firstEmail, string secondEmail)
    {
        // Arrange
        RecreateDbContext();

        
        Context.Users.Add(User.Create("123", firstEmail, "123", 1));
        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        var request = new RegisterCommand("Boban", secondEmail, "12345678");

        var handler = new RegisterCommandHandler(Context, HashProvider, JwtProvider);

        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsFaulted.Should().BeTrue();
        
        var exception = result.Match<Exception>(
            success => new Exception(),
            failure => failure
            );
        
        exception.Should().BeOfType<UserWithTheSameEmailException>();
    }
    
    [Fact]
    public void Register_Should_ReturnUserResponse_OnValidRequest()
    {
        // Arrange
        RecreateDbContext();

        Context.Users.Add(User.Create("123", "blabla@mail.ru", "123", 1));
        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        var request = new RegisterCommand("Boban", "bimbimbim@bambam.ru", "12345678");

        var handler = new RegisterCommandHandler(Context, HashProvider, JwtProvider);

        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var response = result.Match<UserResponse>(
            success => success,
            failure => null
        );

        response.Should().Match<UserResponse>(
            x => !string.IsNullOrWhiteSpace(x.RefreshToken) &&
                 !string.IsNullOrWhiteSpace(x.AccessToken) &&
                 x.Username == request.Name
            );
    }
}