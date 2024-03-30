using Domain.Favorites;
using FluentAssertions;

namespace DomainTests;

public class FavoriteTests
{
    [Fact]
    public void Creat_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        
        // Act
        var action = () => Favorite.Create(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        action.Should().NotThrow();
    }
}