using Application.Authentication.RequestVerification;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;

namespace ApplicationTests.Authentication;

public class RequestVerificationQueryHandlerTests
    : AuthTestingBase<RequestVerificationQueryHandlerTests>
{
    [Fact]
    public void RequestVerification_Should_PublishNewUserCreatedDomainEvent_OnValidUser()
    {
        // Arrange
        RecreateDbContext();

        var user = User.Create(
            "teststest",
            "testemail@123.ru",
            "12312312312312",
            Role.DefaultUser
            );
        
        Context.Users.Add(user);

        Context.SaveChangesAsync().GetAwaiter().GetResult();

        var query = new RequestVerificationQuery(user.Id);
        
        var handler = new RequestVerificationQueryHandler(Context);
        
        // Act
        var result = handler.Handle(query, default).GetAwaiter().GetResult();
        
        // Assert
        Context.OutboxMessages.Count().Should().Be(2);
    }

    [Fact]
    public void RequestVerification_Should_ReturnUserNotFoundException_OnNonexistentUser()
    {
        // Arrange
        RecreateDbContext();

        var user = User.Create(
            "teststest",
            "testemail@123.ru",
            "12312312312312",
            Role.DefaultUser
        );

        var query = new RequestVerificationQuery(user.Id);
        
        var handler = new RequestVerificationQueryHandler(Context);
        
        // Act
        var result = handler.Handle(query, default).GetAwaiter().GetResult();
        
        // Assert
        var exception = result.Match(
            s => new Exception(),
            f => f
        );

        exception.Should().BeOfType<UserNotFoundException>();
    }
    
    [Fact]
    public void RequestVerification_Should_ReturnEmailAlreadyVerifiedException_OnVerifiedUser()
    {
        // Arrange
        RecreateDbContext();

        var user = User.Create(
            "teststest",
            "testemail@123.ru",
            "12312312312312",
            Role.DefaultUser
        );
        
        user.VerifyEmail();
        
        Context.Users.Add(user);

        Context.SaveChangesAsync().GetAwaiter().GetResult();

        var query = new RequestVerificationQuery(user.Id);
        
        var handler = new RequestVerificationQueryHandler(Context);
        
        // Act
        var result = handler.Handle(query, default).GetAwaiter().GetResult();
        
        // Assert
        var exception = result.Match(
            s => new Exception(),
            f => f
        );

        exception.Should().BeOfType<EmailAlreadyVerifiedException>();
    }
}