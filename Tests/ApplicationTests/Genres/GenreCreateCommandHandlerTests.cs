using Application.Genres.CreateGenre;
using Domain.Tracks;
using Domain.Tracks.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Genres;

public class GenreCreateCommandHandlerTests : CatalogTestingBase
{
    public GenreCreateCommandHandlerTests() : base(typeof(GenreCreateCommandHandlerTests))
    {
    }

    [Fact]
    public void CreateGenre_Should_ReturnException_OnDuplicateName()
    {
        // Arrange
        RecreateDbContext();

        string oldName = "Rock";
        string newName = "rock";
        
        var request1 = new CreateGenreCommand(oldName);

        var handler = new CreateGenreCommandHandler(Context);
        
        var result1 = handler.Handle(request1, default).GetAwaiter().GetResult();
        
        // Act
        var request2 = new CreateGenreCommand(newName);
        
        var result2 = handler.Handle(request2, default).GetAwaiter().GetResult();

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeFalse();

        var exception = result2.Match(
            success => new Exception(),
            failure => failure
        );
        
        exception.Should().BeOfType<GenreWithSameNameException>();
    }
    
    [Fact]
    public void CreateGenre_Should_ReturnTrue_OnValidName()
    {
        // Arrange
        RecreateDbContext();

        string name = "Rock";
        
        var request = new CreateGenreCommand(name);

        var handler = new CreateGenreCommandHandler(Context);
        
        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}