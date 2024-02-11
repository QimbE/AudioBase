using Domain.Tracks;
using FluentAssertions;

namespace DomainTests;

public class GenreTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceName(string? value)
    {
        // Arrange
        
        // Act
        var action = () => Genre.Create(value);
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Update_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        var genre = Genre.Create("Pop");
        var expResult = Genre.Create("Rock");
        // Act
        var action = () => genre.Update("Rock");
        // Assert
        action.Should().NotThrow();

        genre.Should().Match<Genre>(g =>
            g.Name == expResult.Name
        );
    }
}