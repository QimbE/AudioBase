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
        // Act
        var response = await HttpClient.PutAsync("Authentication/Refresh", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Refresh_Should_ReturnBadRequest_IfRefreshTokenIsInvalid()
    {
        // Arrange
        var fakeToken = "bimbimbimbambambam";
        
        var user = User.Create("bimbim", "bambam", "123123", Role.DefaultUser);
        
        HttpClient.DefaultRequestHeaders.Add("cookie", [$"refreshToken={fakeToken}"]);
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        // Act
        var response = await HttpClient.PutAsync("Authentication/Refresh", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Refresh_Should_ReturnTokenResponse_IfRefreshTokenValid()
    {
        // Arrange
        var user = User.Create("bimbim", "bambam", "123123", Role.DefaultUser);

        user.RefreshToken.Update("123123123");
        user.VerifyEmail();
        
        var actualToken = user.RefreshToken.Value;
        
        HttpClient.DefaultRequestHeaders.Add("cookie", [$"refreshToken={actualToken}"]);
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        // Act
        var response = await HttpClient.PutAsync("Authentication/Refresh", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content.Should().NotBeNull();
        
        var responseWrapper = await response.Content.ReadFromJsonAsync<ResponseWithData<TokenResponse>>();
        
        responseWrapper.Should().NotBeNull();

        var content = responseWrapper.Data;

        content.AccessToken.Should().NotBeNull();
    }
}