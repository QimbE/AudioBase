using Application.Users.ForgotPassword;
using ApplicationTests.Authentication;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Users;

public class ForgotPasswordQueryHandlerTests
    : AuthTestingBase<ForgotPasswordQueryHandlerTests>
{
    [Fact]
    public void ForgotPassword_ShouldReturn_UserNotFoundException_OnNonexistentUser()
    {
        // Arrange
        RecreateDbContext();

        var fakeEmail = "someEmail1@mail.ru";

        var query = new ForgotPasswordQuery(fakeEmail);

        var handler = new ForgotPasswordQueryHandler(Context);
        
        // Act
        var result = handler.Handle(query, default).GetAwaiter().GetResult();
        
        // Assert
        result.IsSuccess.Should().BeFalse();

        var exception = result.Match(
            success => new Exception(),
            fail => fail
        );

        exception.Should().BeOfType<UserNotFoundException>();
    }
    
    [Fact]
    public void ForgotPassword_ShouldReturn_True_OnExistentUser()
    {
        // Arrange
        RecreateDbContext();
        var user = User.Create("bimbim", "bam@bam.ru", "BimBam123", Role.CatalogAdmin);
        
        var request = new ForgotPasswordQuery(user.Email);
        
        var handler = new ForgotPasswordQueryHandler(Context);

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