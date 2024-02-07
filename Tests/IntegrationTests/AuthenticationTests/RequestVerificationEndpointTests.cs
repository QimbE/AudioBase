using System.Net;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.AuthenticationTests;

public class RequestVerificationEndpointTests: BaseIntegrationTest
{
    public RequestVerificationEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task RequestVerification_ShouldReturn_NotFound_OnNonexistentUser()
    {
        // Arrange
        var httpClient = Factory.CreateClient();
        
        // Act
        var response = await httpClient.GetAsync($"Authentication/RequestVerification?userId={Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task RequestVerification_ShouldReturn_BadRequest_OnVerifiedUser()
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        var user = User.Create(
            "123123123123",
            "12312hehehe@huh.ru",
            "123123123",
            Role.DefaultUser
        );
        user.VerifyEmail();
        
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(user);
        await context.SaveChangesAsync();
        
        // Act
        var response = await httpClient.GetAsync($"Authentication/RequestVerification?userId={user.Id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await RecreateDatabase();
    }
    
    [Fact]
    public async Task RequestVerification_ShouldReturn_Ok_OnUnverifiedUser()
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        var user = User.Create(
            "123123123123",
            "12312hehehe@huh.ru",
            "123123123",
            Role.DefaultUser
        );
        
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(user);
        await context.SaveChangesAsync();
        
        // Act
        var response = await httpClient.GetAsync($"Authentication/RequestVerification?userId={user.Id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        await RecreateDatabase();
    }
}