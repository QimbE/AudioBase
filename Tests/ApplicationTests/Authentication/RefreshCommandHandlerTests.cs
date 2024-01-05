using Application.Authentication.Refresh;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Authentication;

public class RefreshCommandHandlerTests: AuthTestingBase
{
    public static IEnumerable<object[]> InvalidUsersAndTokens()
    {
        var user1 = User.Create("123", "test", "123123", 1);
        
        // Expired token
        var expiredToken = RefreshToken.Create("123123", user1.Id);
        expiredToken.MakeExpire();
        
        // Token that does not exist
        var nonExistentToken = RefreshToken.Create("123", Guid.NewGuid());
        
        return [[user1, expiredToken], [user1, nonExistentToken]];
    }
    
    [Theory]
    [MemberData(nameof(InvalidUsersAndTokens))]
    public async Task Refresh_ShouldReturnException_OnInvalidToken(User user, RefreshToken token)
    {
        // Arrange
        await RecreateDbContextAsync();
        Context.Users.Add(user);
        
        if (token.Id == user.Id)
        {
            Context.RefreshTokens.Add(token);
        }

        await Context.SaveChangesAsync();

        var handler = new RefreshCommandHandler(Context, JwtProvider);

        var request = new RefreshCommand(token.Value);
        
        // Act
        var res = await handler.Handle(request, default);
        
        // Assert
        res.IsFaulted.Should().BeTrue();

        var exception = res.Match<Exception>(
            success => null,
            failure => failure
            );

        exception.Should().BeOfType<InvalidRefreshTokenException>();
    }
    
    [Fact]
    public async Task Refresh_ShouldReturnTokenResponse_OnValidToken()
    {
        // Arrange
        await RecreateDbContextAsync();
        
        var user = User.Create("123", "test", "123123", 1);

        var token = RefreshToken.Create("123123123123", user.Id);
        
        Context.Users.Add(user);
        Context.RefreshTokens.Add(token);
        
        await Context.SaveChangesAsync();

        var handler = new RefreshCommandHandler(Context, JwtProvider);

        var request = new RefreshCommand(token.Value);
        
        // Act
        var res = await handler.Handle(request, default);
        
        // Assert
        res.IsSuccess.Should().BeTrue();

        var exception = res.Match<TokenResponse>(
            success => success,
            failure => null
        );

        exception.Should().Match<TokenResponse>(
            t => !string.IsNullOrWhiteSpace(t.AccessToken)
            );
    }
}