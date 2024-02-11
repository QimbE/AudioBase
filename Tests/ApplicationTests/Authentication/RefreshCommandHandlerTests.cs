using Application.Authentication.Refresh;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Authentication;

public class RefreshCommandHandlerTests: AuthTestingBase<RefreshCommandHandlerTests>
{
    public static IEnumerable<object[]> InvalidUsersAndTokens()
    {
        var user1 = User.Create("123", "test", "123123", 1);
        
        // Token that does not exist
        var nonExistentToken = RefreshToken.Create("123", Guid.NewGuid());
        
        return [[user1, user1.RefreshToken], [user1, nonExistentToken]];
    }
    
    [Theory]
    [MemberData(nameof(InvalidUsersAndTokens))]
    public void Refresh_ShouldReturnException_OnInvalidToken(User user, RefreshToken token)
    {
        // Arrange
        RecreateDbContext();
        Context.Users.Add(user);
        
        if (token.Id == user.Id)
        {
            Context.RefreshTokens.Add(token);
        }

        Context.SaveChangesAsync().GetAwaiter().GetResult();

        var handler = new RefreshCommandHandler(Context, JwtProvider);

        var request = new RefreshCommand(token.Value);
        
        // Act
        var res = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        res.IsFaulted.Should().BeTrue();

        var exception = res.Match<Exception>(
            success => null,
            failure => failure
            );

        exception.Should().BeOfType<InvalidRefreshTokenException>();
    }
    
    [Fact]
    public void Refresh_ShouldReturnTokenResponse_OnValidToken()
    {
        // Arrange
        RecreateDbContext();
        
        var user = User.Create("123", "test", "123123", 1);
        
        user.VerifyEmail();
        
        user.RefreshToken.Update(JwtProvider.GenerateRefreshToken());
        Context.Users.Add(user);
        
        Context.SaveChangesAsync().GetAwaiter().GetResult();

        var handler = new RefreshCommandHandler(Context, JwtProvider);

        var request = new RefreshCommand(user.RefreshToken.Value);
        
        // Act
        var res = handler.Handle(request, default).GetAwaiter().GetResult();
        
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