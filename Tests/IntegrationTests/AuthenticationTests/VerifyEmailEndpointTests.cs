using System.Net;
using Application.Authentication;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Endpoints;

namespace IntegrationTests.AuthenticationTests;


public class VerifyEmailEndpointTests: BaseIntegrationTest
{
    private readonly User _defaultUser = User.Create(
        "Test",
        "test@test.ru",
        "123123",
        Role.List.First()
    );
    
    public VerifyEmailEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        
    }

    [Fact]
    public async Task VerifyEmail_Should_ReturnUnauthorized_OnInvalidToken()
    {
        // Arrange
        await RecreateDatabase();
        var httpClient = Factory.CreateClient();
        
        using var scope = Factory.Services.CreateScope();
        
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var token = await jwtProvider.GenerateAccessToken(_defaultUser)+"123";
        
        // Act
        var response = await httpClient.GetAsync($"{nameof(Authentication)}/VerifyEmail?token={token}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task VerifyEmail_Should_ReturnNotFound_OnNonexistentUser()
    {
        // Arrange
        await RecreateDatabase();
        var httpClient = Factory.CreateClient();
        
        using var scope = Factory.Services.CreateScope();
        
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var token = await jwtProvider.GenerateVerificationToken(_defaultUser);
        
        // Act
        var response = await httpClient.GetAsync($"{nameof(Authentication)}/VerifyEmail?token={token}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task VerifyEmail_Should_ReturnOk_OnValidRequest()
    {
        // Arrange
        var httpClient = Factory.CreateClient();
        
        using var scope = Factory.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_defaultUser);

        await context.SaveChangesAsync();
        
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var token = await jwtProvider.GenerateVerificationToken(_defaultUser);
        
        // Act
        var response = await httpClient.GetAsync($"{nameof(Authentication)}/VerifyEmail?token={token}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}