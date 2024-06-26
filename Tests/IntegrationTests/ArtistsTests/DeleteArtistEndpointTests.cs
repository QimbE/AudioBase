using System.Net;
using System.Net.Http.Json;
using Application.Artists.DeleteArtist;
using Application.Authentication;
using Domain.Artists;
using Domain.Junctions;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.ArtistsTests;

public class DeleteArtistEndpointTests: BaseIntegrationTest
{
    public DeleteArtistEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task DeleteArtistEndpoint_Should_ReturnBadRequest_OnNullId()
    {
        // Arrange
        var createName = "MF DOOM";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteArtistCommand(new Guid());
        
        // Act
        // Not sure why but HttpClient.DeleteFromJsonAsync() does not work due to unavailability of setting request as content
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Artists/DeleteArtist") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var tryToFind = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteArtistEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var createName = "MF DOOM";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.DefaultUser);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteArtistCommand(artistByName.Id);
        
        // Act
        // Not sure why but HttpClient.DeleteFromJsonAsync() does not work due to unavailability of setting request as content
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Artists/DeleteArtist") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        var tryToFind = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteArtistEndpoint_Should_ReturnNotFound_OnNonexistentArtist()
    {
        // Arrange
        var createName = "MF DOOM";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteArtistCommand(Guid.NewGuid());
        
        // Act
        // Not sure why but HttpClient.DeleteFromJsonAsync() does not work due to unavailability of setting request as content
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Artists/DeleteArtist") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var tryToFind = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteArtistEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var createName = "MF DOOM";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Artists.Add(Artist.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var artistByName = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteArtistCommand(artistByName.Id);
        
        // Act
        // Not sure why but HttpClient.DeleteFromJsonAsync() does not work due to unavailability of setting request as content
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Artists/DeleteArtist") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
        
        var tryToFind = context.Artists.SingleOrDefaultAsync(a => a.Name == createName).Result;

        tryToFind.Should().BeNull();
    }
}