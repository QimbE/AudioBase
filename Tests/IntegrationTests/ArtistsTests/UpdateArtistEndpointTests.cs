using System.Net;
using System.Net.Http.Json;
using Application.Artists.UpdateArtist;
using Application.Authentication;
using Domain.Artists;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.ArtistsTests;

public class UpdateArtistEndpointTests: BaseIntegrationTest
{
    public UpdateArtistEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task UpdateArtistEndpoint_Should_ReturnBadRequest_OnNullId()
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        string newName = "NewName";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(new Guid(), newName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("0123456789 0123456789 0123456789 0123456789 0123456789 0123456789")] // Exceeds max size
    public async Task UpdateArtistEndpoint_Should_ReturnBadRequest_OnInvalidArtistName(string artistName)
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(artistByName.Id, artistName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateArtistEndpoint_Should_ReturnBadRequest_OnInvalidDescription()
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        String tooLarge = String.Concat(Enumerable.Repeat("0123456789", 201)); // Exceeds max size
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(artistByName.Id, createName, tooLarge, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateArtistEndpoint_Should_ReturnBadRequest_OnInvalidPhotoLink(string photoLink)
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(artistByName.Id, createName, createDesc, photoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateArtistEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        string newName = "NewName";
        string newPhotoLink = "https://rofliks.com/roflan.webp";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.DefaultUser);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(artistByName.Id, newName, createDesc, newPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task UpdateArtistEndpoint_Should_ReturnNotFound_OnNonexistentArtist()
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        string newName = "NewName";
        string newPhotoLink = "https://rofliks.com/roflan.webp";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(Guid.NewGuid(), newName, createDesc, newPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateArtistEndpoint_Should_ReturnConflict_OnEqualData()
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(artistByName.Id, createName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task UpdateArtistEndpoint_Should_ReturnConflict_OnDuplicateName()
    {
        // Arrange
        string toChangeName = "toChangeName";
        string toFindName = "toFindName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(toChangeName, createDesc, createPhotoLink));
        context.Artists.Add(Artist.Create(toFindName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == toChangeName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(artistByName.Id, toFindName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task UpdateArtistEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        string newName = "NewName";
        string newPhotoLink = "https://rofliks.com/roflan.webp";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateArtistCommand(artistByName.Id, newName, createDesc, newPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Artists/UpdateArtist", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}