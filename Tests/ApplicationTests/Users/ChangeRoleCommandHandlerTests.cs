using Application.Users.ChangeRole;
using ApplicationTests.Authentication;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Users;

public class ChangeRoleCommandHandlerTests
    : AuthTestingBase<ChangeRoleCommandHandlerTests>
{
    [Fact]
    public void ChangeRole_Should_ReturnException_OnInvalidId()
    {
        // Arrange
        RecreateDbContext();
        var request = new ChangeRoleCommand(Guid.NewGuid(), "Admin");
        
        var handler = new ChangeRoleCommandHandler(Context);
        
        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match(
            success => new Exception(),
            fail => fail
            );

        exception.Should().BeOfType<UserNotFoundException>();
    }

    [Fact]
    public void ChangeRole_Should_ReturnTrue_OnValidRequest()
    {
        // Arrange
        RecreateDbContext();
        var user = User.Create("bimbim", "bam@bam.ru", "BimBam123", Role.CatalogAdmin);
        
        var request = new ChangeRoleCommand(user.Id, "Admin");
        
        var handler = new ChangeRoleCommandHandler(Context);

        Context.Users.Add(user);

        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var success = result.Match(
            succ => succ,
            fail => false
            );

        success.Should().BeTrue();
    }
}