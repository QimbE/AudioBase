using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Genres.CreateGenre;
using Application.Users.ChangeRole;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.GenresTests;

public class CreateGenreEndpointTests: BaseIntegrationTest
{
    public CreateGenreEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateGenreEndpoint_Should_ReturnBadRequest_OnInvalidGenreName(string genreName)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.Admin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateGenreCommand(genreName);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Genres/CreateGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateGenreEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.DefaultUser);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateGenreCommand("Rock");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Genres/CreateGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateGenreEndpoint_Should_ReturnConflict_OnDuplicateGenreName()
    {
        // Arrange
        string createName = "Hip-Hop";
        
        string newName = "Hip-Hop";
        
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.Admin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateGenreCommand(createName);
        
        var response = await HttpClient.PostAsJsonAsync("Genres/CreateGenre", request);
        
        request = new CreateGenreCommand(newName);
        
        // Act
        response = await HttpClient.PostAsJsonAsync("Genres/CreateGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task CreateGenreEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.Admin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateGenreCommand("Hip-Hop");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Genres/CreateGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}