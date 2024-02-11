using Domain.Tracks;
using FluentAssertions;

namespace DomainTests;

public class TrackTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceName(string? value)
    {
        // Arrange
        
        // Act
        var action = () => Track.Create(value, "https://youtu.be/dQw4w9WgXcQ", new TimeSpan(0,0,5,32), new Guid(), new Guid());
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceAudioLink(string? value)
    {
        // Arrange
        
        // Act
        var action = () => Track.Create("Name", value, new TimeSpan(0,0,5,32), new Guid(), new Guid());
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_OnZeroDuration()
    {
        // Arrange
        
        // Act
        var action = () => Track.Create("Name", "https://youtu.be/dQw4w9WgXcQ", new TimeSpan(), new Guid(), new Guid());
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Update_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        Guid rGuid = new Guid();
        Guid gGuid = new Guid();
        var track = Track.Create("Name","https://youtu.be/dQw4w9WgXcQ", new TimeSpan(0,0,5,30), new Guid(), new Guid());
        var expResult = Track.Create("NewName","http://youtu.be/dQw4w9WgXcQ", new TimeSpan(0,1,5,30), rGuid, gGuid);
        // Act
        var action = () => track.Update("NewName","http://youtu.be/dQw4w9WgXcQ", new TimeSpan(0,1,5,30), rGuid, gGuid);
        // Assert
        action.Should().NotThrow();

        track.Should().Match<Track>(t =>
            t.Name == expResult.Name &&
            t.AudioLink == expResult.AudioLink &&
            t.Duration == expResult.Duration &&
            t.ReleaseId == expResult.ReleaseId &&
            t.GenreId == expResult.GenreId
        );
    }
}