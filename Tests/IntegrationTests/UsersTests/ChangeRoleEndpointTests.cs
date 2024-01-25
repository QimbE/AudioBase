using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Users.ChangeRole;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.UsersTests;

public class ChangeRoleEndpointTests: BaseIntegrationTest
{
    public ChangeRoleEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ChangeRoleEndpoint_Should_ReturnBadRequest_OnInvalidRoleName()
    {
        // Arrange
        var httpClient = Factory.CreateClient();
        
        using var scope = Factory.Services.CreateScope();
        
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bim", "Bombom", "123123123", Role.Admin);

        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        httpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new ChangeRoleCommand(user.Id, "qpowejpwkjlkasdasdasa");
        
        // Act
        var response = await httpClient.PatchAsJsonAsync("User/ChangeRole", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ChangeRoleEndpoint_Should_ReturnNotFound_OnNonexistentUserId()
    {
        // Arrange
        var httpClient = Factory.CreateClient();
        
        using var scope = Factory.Services.CreateScope();
        
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bim", "Bombom", "123123123", Role.Admin);

        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        httpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new ChangeRoleCommand(user.Id, Role.Admin.Name);
        
        // Act
        var response = await httpClient.PatchAsJsonAsync("User/ChangeRole", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ChangeRoleEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var httpClient = Factory.CreateClient();
        
        using var scope = Factory.Services.CreateScope();
        
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bim", "Bombom", "123123123", Role.Admin);

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        httpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new ChangeRoleCommand(user.Id, Role.CatalogAdmin.Name);
        
        // Act
        var response = await httpClient.PatchAsJsonAsync("User/ChangeRole", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}