using Domain.Junctions;
using FluentAssertions;

namespace DomainTests;

public class LabelReleaseTests
{
    [Fact]
    public void Creat_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        
        // Act
        var action = () => LabelRelease.Create(Guid.NewGuid(), Guid.NewGuid());
        // Assert
        action.Should().NotThrow();
    }
}