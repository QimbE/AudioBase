using System.Security.Authentication;
using Application.Users.ChangePassword;
using ApplicationTests.Authentication;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Users;

public class ChangePasswordCommandHandlerTests
    : AuthTestingBase
{
    public ChangePasswordCommandHandlerTests()
        : base(typeof(ChangePasswordCommandHandlerTests))
    {
    }

    [Fact]
    public void ChangePassword_Should_ReturnException_OnInvalidUserId()
    {
        // Arrange
        RecreateDbContext();

        var request = new ChangePasswordCommand(Guid.NewGuid(), "123123123123123123", "bimbibmibmbimb");

        var handler = new ChangePasswordCommandHandler(Context, HashProvider);

        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match(
            success => new Exception(),
            failure => failure
            );
        
        exception.Should().BeOfType<UserNotFoundException>();
    }
    
    [Fact]
    public void ChangePassword_Should_ReturnException_OnInvalidPassword()
    {
        // Arrange
        RecreateDbContext();

        var user = User.Create(
            "123123123",
            "bimbimb@bam.ru",
            HashProvider.HashPassword("hehehehuh").GetAwaiter().GetResult(),
            Role.Admin
            );

        Context.Users.Add(user);

        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        var request = new ChangePasswordCommand(user.Id, "123123123123123123", "bimbibmibmbimb");

        var handler = new ChangePasswordCommandHandler(Context, HashProvider);

        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match(
            success => new Exception(),
            failure => failure
        );
        
        exception.Should().BeOfType<InvalidCredentialException>();
    }
    
    [Fact]
    public void ChangePassword_Should_ReturnException_OnDuplicatePassword()
    {
        // Arrange
        RecreateDbContext();

        var password = "bimBamBimBim123";
        
        var user = User.Create(
            "123123123",
            "bimbimb@bam.ru",
            HashProvider.HashPassword(password).GetAwaiter().GetResult(),
            Role.Admin
            );

        Context.Users.Add(user);

        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        var request = new ChangePasswordCommand(user.Id, password, password);

        var handler = new ChangePasswordCommandHandler(Context, HashProvider);

        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match(
            success => new Exception(),
            failure => failure
        );
        
        exception.Should().BeOfType<ChangeToTheSamePasswordException>();
    }
    
    [Fact]
    public void ChangePassword_Should_ReturnTrue_OnValidRequest()
    {
        // Arrange
        RecreateDbContext();

        var password = "bimBamBimBim123";
        
        var user = User.Create(
            "123123123",
            "bimbimb@bam.ru",
            HashProvider.HashPassword(password).GetAwaiter().GetResult(),
            Role.Admin
        );

        Context.Users.Add(user);

        Context.SaveChangesAsync().GetAwaiter().GetResult();
        
        var request = new ChangePasswordCommand(user.Id, password, "123123123123123123ppsdppp");

        var handler = new ChangePasswordCommandHandler(Context, HashProvider);

        // Act
        var result = handler.Handle(request, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeTrue();

        var suc = result.Match(
            success => success,
            failure => false
        );

        suc.Should().BeTrue();

        var changedUser = Context.Users.SingleOrDefault(u => u.Id == user.Id);

        HashProvider
            .VerifyPassword(request.NewPassword, changedUser.Password)
            .GetAwaiter()
            .GetResult()
            .Should()
            .BeTrue();
    }
}