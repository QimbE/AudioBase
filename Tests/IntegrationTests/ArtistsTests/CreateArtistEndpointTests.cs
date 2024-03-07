using System.Net;
using System.Net.Http.Json;
using Application.Artists.CreateArtist;
using Application.Authentication;
using Domain.Artists;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.ArtistsTests;

public class CreateArtistEndpointTests: BaseIntegrationTest
{
    public CreateArtistEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("0123456789 0123456789 0123456789 0123456789 0123456789 0123456789")] // Exceeds max size
    public async Task CreateArtistEndpoint_Should_ReturnBadRequest_OnInvalidArtistName(string artistName)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateArtistCommand(artistName, "rofliks", "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Artists/CreateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateArtistEndpoint_Should_ReturnBadRequest_OnInvalidArtistDescription()
    {
        // Arrange
        String tooLarge = String.Concat(Enumerable.Repeat("0123456789", 201)); // Exceeds max size
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateArtistCommand("MF DOOM", tooLarge, "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Artists/CreateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateArtistEndpoint_Should_ReturnBadRequest_OnInvalidPhotoLink(string photoLink)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateArtistCommand("MF DOOM", "rofliks", photoLink);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Artists/CreateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateArtistEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.DefaultUser);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateArtistCommand("MF DOOM", "rofliks", "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Artists/CreateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateArtistEndpoint_Should_ReturnConflict_OnDuplicateArtistName()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var firstArtist = Artist.Create("MF DOOM", "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");

        context.Users.Add(user);

        context.Artists.Add(firstArtist);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);

        var request = new CreateArtistCommand("MF DOOM", "roflanchik", "https://rofliks.com/rofl.gif");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Artists/CreateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task CreateArtistEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateArtistCommand("MF DOOM", "rofliks", "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Artists/CreateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}