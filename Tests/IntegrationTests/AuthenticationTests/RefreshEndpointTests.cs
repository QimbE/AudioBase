using System.Net;
using System.Net.Http.Json;
using Application.Authentication.Refresh;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.AuthenticationTests;

public class RefreshEndpointTests: BaseIntegrationTest
{
    public RefreshEndpointTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task Refresh_Should_ReturnUnauthorized_IfThereIsNoRefreshToken()
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        // Act
        var response = await httpClient.PutAsync("Authentication/Refresh", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Refresh_Should_ReturnBadRequest_IfRefreshTokenIsInvalid()
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        var fakeToken = "bimbimbimbambambam";
        
        var user = User.Create("bimbim", "bambam", "123123", Role.DefaultUser);
        
        httpClient.DefaultRequestHeaders.Add("cookie", [$"refreshToken={fakeToken}"]);

        using var scope = Factory.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        // Act
        var response = await httpClient.PutAsync("Authentication/Refresh", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
    
    [Fact]
    public async Task Refresh_Should_ReturnTokenResponse_IfRefreshTokenValid()
    {
        // Arrange
        var httpClient = Factory.CreateClient();
        
        var user = User.Create("bimbim", "bambam", "123123", Role.DefaultUser);

        user.RefreshToken.Update("123123123");
        user.VerifyEmail();
        
        var actualToken = user.RefreshToken.Value;
        
        httpClient.DefaultRequestHeaders.Add("cookie", [$"refreshToken={actualToken}"]);

        using var scope = Factory.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        // Act
        var response = await httpClient.PutAsync("Authentication/Refresh", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content.Should().NotBeNull();
        
        var responseWrapper = await response.Content.ReadFromJsonAsync<ResponseWithData<TokenResponse>>();
        
        responseWrapper.Should().NotBeNull();

        var content = responseWrapper.Data;

        content.AccessToken.Should().NotBeNull();
        
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}