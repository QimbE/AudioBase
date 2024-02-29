using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Genres.CreateGenre;
using Application.Genres.RenameGenre;
using Application.Users.ChangeRole;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.GenresTests;

public class RenameGenreEndpointTests: BaseIntegrationTest
{
    public RenameGenreEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RenameGenreEndpoint_Should_ReturnBadRequest_OnInvalidGenreName(string genreName)
    {
        // Arrange
        string createName = "Hip-Hop";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.Admin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Genres.Add(Genre.Create(createName));

        await context.SaveChangesAsync();

        var genreByName = context.Genres.SingleOrDefaultAsync(g => g.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new RenameGenreCommand(genreByName.Id ,genreName);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Genres/RenameGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task RenameGenreEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        string createName = "Hip-Hop";
        
        string newName = "Rock";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.DefaultUser);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Genres.Add(Genre.Create(createName));

        await context.SaveChangesAsync();

        var genreByName = context.Genres.SingleOrDefaultAsync(g => g.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new RenameGenreCommand(genreByName.Id ,newName);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Genres/RenameGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task RenameGenreEndpoint_Should_ReturnNotFound_OnNonExistentGenre()
    {
        // Arrange
        string createName = "Hip-Hop";
        
        string newName = "Rock";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.Admin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Genres.Add(Genre.Create(createName));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new RenameGenreCommand(Guid.NewGuid() , newName);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Genres/RenameGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task RenameGenreEndpoint_Should_ReturnConflict_OnDuplicateGenreName()
    {
        // Arrange
        string createName = "Hip-Hop";
        
        string newName = "hip-hop";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.Admin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Genres.Add(Genre.Create(createName));

        await context.SaveChangesAsync();

        var genreByName = context.Genres.SingleOrDefaultAsync(g => g.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new RenameGenreCommand(genreByName.Id ,newName);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Genres/RenameGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task RenameGenreEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        string createName = "Hip-Hop";
        
        string newName = "Rock";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.Admin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Genres.Add(Genre.Create(createName));

        await context.SaveChangesAsync();

        var genreByName = context.Genres.SingleOrDefaultAsync(g => g.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new RenameGenreCommand(genreByName.Id ,newName);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Genres/RenameGenre", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}