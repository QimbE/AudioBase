using Domain.Labels;
using FluentAssertions;

namespace DomainTests;

public class LabelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceName(string? value)
    {
        // Arrange
        
        // Act
        var action = () => Label.Create(value, "decription", "https://youtu.be/dQw4w9WgXcQ");
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpacePhotoLink(string? value)
    {
        // Arrange
        
        // Act
        var action = () => Label.Create("Name", "Desc", value);
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldNotThrowException_OnNullOrWhiteSpaceDescription(string? value)
    {
        // Arrange
        
        // Act
        var action = () => Label.Create("Name", value, "https://youtu.be/dQw4w9WgXcQ");
        // Assert
        action.Should().NotThrow();
    }
    
    [Fact]
    public void Update_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        var label = Label.Create("Name", "Desc", "https://youtu.be/dQw4w9WgXcQ");
        var expResult = Label.Create("NewName", "NewDesc", "http://youtu.be/dQw4w9WgXcQ");
        // Act
        var action = () => label.Update("NewName", "NewDesc", "http://youtu.be/dQw4w9WgXcQ");
        // Assert
        action.Should().NotThrow();

        label.Should().Match<Label>(l =>
            l.Name == expResult.Name &&
            l.Description == expResult.Description &&
            l.PhotoLink == expResult.PhotoLink
        );
    }
}