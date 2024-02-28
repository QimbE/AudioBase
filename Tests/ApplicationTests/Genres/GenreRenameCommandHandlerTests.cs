using Application.Genres.CreateGenre;
using Application.Genres.RenameGenre;
using Domain.Tracks;
using Domain.Tracks.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests.Genres;

public class GenreRenameCommandHandlerTests:  CatalogTestingBase
{
    public GenreRenameCommandHandlerTests() : base(typeof(GenreRenameCommandHandlerTests))
    {
    }
    
    [Fact]
    public void RenameGenre_Should_ReturnException_OnDuplicateName()
    {
        // Arrange
        RecreateDbContext();

        string createName = "Rock";
        string updateName = "rock";
        
        Context.Genres.Add(Genre.Create(createName));

        Context.SaveChanges();
        
        var g = Context.Genres.SingleOrDefaultAsync(g => g.Name == createName);
        
        // Act
        var request = new RenameGenreCommand(g.Result.Id,updateName);

        var handler = new RenameGenreCommandHandler(Context);
        
        var result = handler.Handle(request, default).GetAwaiter().GetResult();

        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match(
            success => new Exception(),
            failure => failure
        );
        
        exception.Should().BeOfType<GenreWithSameNameException>();
    }
    
    [Fact]
    public void RenameGenre_Should_ReturnException_OnNonExistentId()
    {
        // Arrange
        RecreateDbContext();

        string createName = "Rock";
        string updateName = "rock";
        
        Context.Genres.Add(Genre.Create(createName));

        Context.SaveChanges();
        
        // Act
        var request = new RenameGenreCommand(Guid.NewGuid(), updateName);

        var handler = new RenameGenreCommandHandler(Context);
        
        var result = handler.Handle(request, default).GetAwaiter().GetResult();

        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match(
            success => new Exception(),
            failure => failure
        );
        
        exception.Should().BeOfType<GenreNotFoundException>();
    }
    
    [Fact]
    public void RenameGenre_Should_ReturnTrue_OnValidData()
    {
        // Arrange
        RecreateDbContext();

        string createName = "Rock";
        string updateName = "Pop";
        
        Context.Genres.Add(Genre.Create(createName));

        Context.SaveChanges();
        
        var g = Context.Genres.SingleOrDefaultAsync(g => g.Name == createName);
        
        // Act
        var request = new RenameGenreCommand(g.Result.Id, updateName);

        var handler = new RenameGenreCommandHandler(Context);
        
        var result = handler.Handle(request, default).GetAwaiter().GetResult();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}