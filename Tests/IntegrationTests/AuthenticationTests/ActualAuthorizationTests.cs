﻿using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Users.ChangeRole;
using Domain.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.AuthenticationTests;

/// <summary>
/// Core authentication and authorization logic tests
/// </summary>
public class ActualAuthorizationTests: BaseIntegrationTest
{
    public ActualAuthorizationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }
    
    public static IEnumerable<object[]> GetInvalidRoles()
    {
        yield return [Role.CatalogAdmin];
        yield return [Role.DefaultUser];
    }
    
    [Theory]
    [MemberData(nameof(GetInvalidRoles))]
    public async Task ChangeRoleEndpoint_Should_Return_Forbidden_OnNotAllowedUser(Role notAllowedRole)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bim", "Bombom", "123123123", notAllowedRole);

        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);

        var request = new ChangeRoleCommand(user.Id, "Admin");
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ChangeRole", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ChangeRoleEndpoint_Should_Return_Unauthorized_OnUnauthenticatedUser()
    {
        // Arrange
        var request = new ChangeRoleCommand(Guid.NewGuid(), "Admin");
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ChangeRole", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}