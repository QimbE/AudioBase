using Domain.Artists;
using FluentAssertions;

namespace DomainTests;

public class ArtistTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceName(string? value)
    {
        // Arrange
        
        // Act
        var action = () => Artist.Create(value, "decription", "https://youtu.be/dQw4w9WgXcQ");
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
        var action = () => Artist.Create("Name", "Desc", value);
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
        var action = () => Artist.Create("Name", value, "https://youtu.be/dQw4w9WgXcQ");
        // Assert
        action.Should().NotThrow();
    }
    
    [Fact]
    public void Update_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        var artist = Artist.Create("Name", "Desc", "https://youtu.be/dQw4w9WgXcQ");
        var expResult = Artist.Create("NewName", "NewDesc", "http://youtu.be/dQw4w9WgXcQ");
        // Act
        var action = () => artist.Update("NewName", "NewDesc", "http://youtu.be/dQw4w9WgXcQ");
        // Assert
        action.Should().NotThrow();

        artist.Should().Match<Artist>(a =>
            a.Name == expResult.Name &&
            a.Description == expResult.Description &&
            a.PhotoLink == expResult.PhotoLink
        );
    }
}