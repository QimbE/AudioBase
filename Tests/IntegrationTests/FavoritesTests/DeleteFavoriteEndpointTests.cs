using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Domain.Artists;
using Domain.Favorites;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Endpoints;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.FavoritesTests;

public class DeleteFavoriteEndpointTests: BaseIntegrationTest
{
    public DeleteFavoriteEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    private readonly User _user = User.Create(
        "Test",
        "test@test.ru",
        "123123",
        Role.List.First()
    );

    private static readonly Artist _author = Artist.Create(
        "Artist",
        "Desc",
        "https://photo.link");
    
    private static readonly Genre _genre = Genre.Create("Rap");

    private static readonly Release _release = Release.Create(
        "Release",
        "https://cover.link",
        _author.Id,
        1,
        new DateOnly(2001,12,12));
    
    private static readonly Track _track = Track.Create(
        "Track",
        "https://audio.link",
        new TimeSpan(0, 0, 1, 23),
        _release.Id,
        _genre.Id);
    
    [Fact]
    public async Task DeleteFavoriteEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(_track);

        context.Favorites.Add(Favorite.Create(_user.Id, _track.Id));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        // Act
        var response = await HttpClient.DeleteAsync(
            $"{nameof(Favorites)}/DeleteFavorite?trackId={_track.Id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteFavoriteEndpoint_Should_ReturnBadRequest_OnNullId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(_track);

        context.Favorites.Add(Favorite.Create(_user.Id, _track.Id));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        // Act
        var response = await HttpClient.DeleteAsync(
            $"{nameof(Favorites)}/DeleteFavorite?trackId={new Guid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task DeleteFavoriteEndpoint_Should_ReturnUnauthorized_OnNotAuthorized()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(_track);

        context.Favorites.Add(Favorite.Create(_user.Id, _track.Id));

        await context.SaveChangesAsync();
        
        // Act
        var response = await HttpClient.DeleteAsync(
            $"{nameof(Favorites)}/DeleteFavorite?trackId={_track.Id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task DeleteFavoriteEndpoint_Should_ReturnNotFound_OnNonexistentTrack()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(_track);

        context.Favorites.Add(Favorite.Create(_user.Id, _track.Id));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        // Act
        var response = await HttpClient.DeleteAsync(
            $"{nameof(Favorites)}/DeleteFavorite?trackId={Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteFavoriteEndpoint_Should_ReturnNotFound_OnWrongUser()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(_track);

        context.Favorites.Add(Favorite.Create(_user.Id, _track.Id));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(
            User.Create(
            "NewUser", 
            "newuser@gmail.com", 
            "newuser123", 
            1));
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        // Act
        var response = await HttpClient.DeleteAsync(
            $"{nameof(Favorites)}/DeleteFavorite?trackId={_track.Id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}