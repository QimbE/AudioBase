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
        var action = () => CoAuthor.Create(new Guid(), new Guid());
        // Assert
        action.Should().NotThrow();
    }
}