using Application.Authentication;
using Application.Authentication.Register;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Authentication;

public class RegisterCommandHandlerTests: AuthTestingBase
{
    [Theory]
    [InlineData("email@a.ru", "EmAiL@a.rU")]
    [InlineData("BobaN@ab.ru", "boban@ab.ru")]
    public async Task Register_Should_ReturnException_OnDuplicateEmail(string firstEmail, string secondEmail)
    {
        // Arrange
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();

        
        Context.Users.Add(User.Create("123", firstEmail, "123", 1));
        await Context.SaveChangesAsync();
        
        var request = new RegisterCommand("Boban", secondEmail, "12345678");

        var handler = new RegisterCommandHandler(Context, HashProvider, JwtProvider);

        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsFaulted.Should().BeTrue();
        
        var exception = result.Match<Exception>(
            success => new Exception(),
            failure => failure
            );
        
        exception.Should().BeOfType<UserWithTheSameEmailException>();
    }
    
    [Fact]
    public async Task Register_Should_ReturnUserResponse_OnValidRequest()
    {
        // Arrange
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();

        Context.Users.Add(User.Create("123", "blabla@mail.ru", "123", 1));
        await Context.SaveChangesAsync();
        
        var request = new RegisterCommand("Boban", "bimbimbim@bambam.ru", "12345678");

        var handler = new RegisterCommandHandler(Context, HashProvider, JwtProvider);

        // Act
        var result = await handler.Handle(request, default);
        
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