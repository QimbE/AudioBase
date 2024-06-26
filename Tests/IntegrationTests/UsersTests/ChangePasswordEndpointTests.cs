﻿using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.DataAccess;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Endpoints;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.UsersTests;

public class ChangePasswordEndpointTests: BaseIntegrationTest
{
    public ChangePasswordEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        
    }

    [Fact]
    public async Task ChangePassword_Should_Return_NotFound_OnNonexistentUser()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bim", "Bombom", "123123123", Role.Admin);

        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);

        var request = new ChangePasswordRequest("123123123", "123123123123332");
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ChangePassword", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ChangePassword_Should_Return_BadRequest_OnInvalidPassword()
    {
        // Arrange
        var hashProvider = Scope.ServiceProvider.GetRequiredService<IHashProvider>();

        var password = "bimbimbimBamBamBam";

        var passwordHash = await hashProvider.HashPassword(password);

        
        var user = User.Create("Bim", "Bombom", passwordHash, Role.Admin);
        
        user.VerifyEmail();
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);

        var request = new ChangePasswordRequest("12321332344431", "123123123123332");
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ChangePassword", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    
    [Fact]
    public async Task ChangePassword_Should_Return_Conflict_OnChangeToTheSamePassword()
    {
        // Arrange

        var hashProvider = Scope.ServiceProvider.GetRequiredService<IHashProvider>();

        var password = "bimbimbimBamBamBam";

        var passwordHash = await hashProvider.HashPassword(password);

        
        var user = User.Create("Bim", "Bombom", passwordHash, Role.Admin);
        
        user.VerifyEmail();
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);

        var request = new ChangePasswordRequest(password, password);
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ChangePassword", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task ChangePassword_Should_Return_BaseResponse_OnValidRequest()
    {
        // Arrange
        var hashProvider = Scope.ServiceProvider.GetRequiredService<IHashProvider>();

        var password = "bimbimbimBamBamBam";

        var passwordHash = await hashProvider.HashPassword(password);

        
        var user = User.Create("Bim", "Bombom", passwordHash, Role.Admin);
        
        user.VerifyEmail();
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);

        var request = new ChangePasswordRequest(password, "bimBomBambam123");
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ChangePassword", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}