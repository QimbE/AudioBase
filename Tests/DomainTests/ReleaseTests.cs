using Domain.MusicReleases;
using FluentAssertions;

namespace DomainTests;

public class ReleaseTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceName(string? value)
    {
        // Arrange
        
        // Act
        var action = () => 
            Release.Create(value, "https://youtu.be/dQw4w9WgXcQ", new Guid(), 1, new DateOnly(2002,2,2));
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceCoverLink(string? value)
    {
        // Arrange
        
        // Act
        var action = () => 
            Release.Create("name", value, new Guid(), 1, new DateOnly(2002,2,2));
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    // TODO: Somehow check if there is an author with such GUID
    
    [Fact]
    public void Create_ShouldThrowException_OnDefaultDate()
    {
        // Arrange
        
        // Act
        var action = () => 
            Release.Create("Name", "https://youtu.be/dQw4w9WgXcQ", new Guid(), 1, new DateOnly());
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_OnNonexistentReleaseTypeId()
    {
        // Arrange
        
        // Act
        var action = () => 
            Release.Create("Name", "https://youtu.be/dQw4w9WgXcQ", new Guid(), -100, new DateOnly(2002,2,2));
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Update_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        Guid resGuid = new Guid();
        var release = Release.Create("Name","https://youtu.be/dQw4w9WgXcQ", new Guid(), 1, new DateOnly(2002,2,2));
        var expResult = Release.Create("NewName","http://youtu.be/dQw4w9WgXcQ", resGuid, 2, new DateOnly(2003,3,3));
        // Act
        var action = () => release.Update("NewName","http://youtu.be/dQw4w9WgXcQ", resGuid, 2, new DateOnly(2003,3,3));
        // Assert
        action.Should().NotThrow();

        release.Should().Match<Release>(r =>
            r.Name == expResult.Name &&
            r.CoverLink == expResult.CoverLink &&
            r.AuthorId == expResult.AuthorId &&
            r.ReleaseTypeId == expResult.ReleaseTypeId &&
            r.ReleaseDate == expResult.ReleaseDate
        );
    }
}