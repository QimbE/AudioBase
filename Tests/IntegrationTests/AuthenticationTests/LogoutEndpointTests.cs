using System.Net;
using System.Net.Http.Json;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.AuthenticationTests;

public class LogoutEndpointTests: BaseIntegrationTest
{
    public LogoutEndpointTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        
    }
    
    [Fact]
    public async Task Logout_Should_ReturnUnauthorized_IfThereIsNoRefreshToken()
    {
        // Act
        var response = await HttpClient.PutAsync("Authentication/Logout", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Logout_Should_ReturnBadRequest_IfRefreshTokenIsInvalid()
    {
        // Arrange
        var fakeToken = "bimbimbimbambambam";
        
        var user = User.Create("bimbim", "bambam", "123123", Role.DefaultUser);
        
        HttpClient.DefaultRequestHeaders.Add("cookie", [$"refreshToken={fakeToken}"]);
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        // Act
        var response = await HttpClient.PutAsync("Authentication/Logout", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Logout_Should_ReturnBaseResponse_IfRefreshTokenValid()
    {
        // Arrange
        var actualToken = "hehehehuh";
        
        var user = User.Create("bimbim", "bambam", "123123", Role.DefaultUser);

        user.RefreshToken.Update(actualToken);
        user.VerifyEmail();
        
        HttpClient.DefaultRequestHeaders.Add("cookie", [$"refreshToken={actualToken}"]);
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        // Act
        var response = await HttpClient.PutAsync("Authentication/Logout", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content.Should().NotBeNull();
        
        var responseWrapper = await response.Content.ReadFromJsonAsync<BaseResponse>();
        
        responseWrapper.Should().NotBeNull();
    }
}