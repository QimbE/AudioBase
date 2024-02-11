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
        var action = () => LabelRelease.Create(new Guid(), new Guid());
        // Assert
        action.Should().NotThrow();
    }
}