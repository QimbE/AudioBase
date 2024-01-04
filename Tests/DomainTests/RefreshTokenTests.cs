using Domain.Users;
using FluentAssertions;
using Throw;

namespace DomainTests;

public class RefreshTokenTests
{
    [Fact]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceValue()
    {
        // Arrange
        var action = () => RefreshToken.Create("    ", Guid.NewGuid());
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ShouldNotThrowException_OnValidValue()
    {
        // Arrange
        var token = RefreshToken.Create("random123", Guid.NewGuid());
        var validString = "SomeValidString";
        
        var action = () =>
        {
            token.Update(validString);
        };

        // Assert
        action.Should().NotThrow<ArgumentException>();

        token.Value.Should().BeEquivalentTo(validString);
    }

    [Fact]
    public void MakeExpire_Should_MakeTokenExpire()
    {
        // Arrange
        var token = RefreshToken.Create("random123", Guid.NewGuid());
        
        // Act
        token.MakeExpire();
        
        // Assert
        token.ExpirationDate.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }
}