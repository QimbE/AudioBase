using Domain.Junctions;
using FluentAssertions;

namespace DomainTests;

public class CoAuthorTests
{
    [Fact]
    public void Creat_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        
        // Act
        var action = () => CoAuthor.Create(Guid.NewGuid(), Guid.NewGuid());
        // Assert
        action.Should().NotThrow();
    }
}