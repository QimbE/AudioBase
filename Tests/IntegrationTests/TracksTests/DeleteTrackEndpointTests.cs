using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Tracks.DeleteTrack;
using Domain.Artists;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.TracksTests;

public class DeleteTrackEndpointTests: BaseIntegrationTest
{
    public DeleteTrackEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    private readonly User _user = User.Create(
        "Test",
        "test@test.ru",
        "123123",
        Role.CatalogAdmin
    );
    
    private (Release, Artist) GetRelease  {
        get
        {
            var art = Artist.Create(
                "Release",
                "Desc",
                "https://photo.link");
            var rel = Release.Create(
                "Release",
                "link",
                art.Id,
                1,
                new DateOnly(2011, 12, 12));
            return (rel, art);
        }
    }
    
    private readonly Genre _genre = Genre.Create("Rap");
    
    [Fact]
    public async Task DeleteTrackEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);
        
        var track = Track.Create("TrackName",
            "AudioLink",
            new TimeSpan(0, 0, 12),
            _release.Id,
            _genre.Id);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteTrackCommand(track.Id);
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Tracks/DeleteTrack") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteTrackEndpoint_Should_ReturnBadRequest_OnNullId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteTrackCommand(new Guid());
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Tracks/DeleteTrack") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task DeleteTrackEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var newUser = User.Create("Name", "email@mail.com", "password123", Role.DefaultUser);
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);
        
        var track = Track.Create("TrackName",
            "AudioLink",
            new TimeSpan(0, 0, 12),
            _release.Id,
            _genre.Id);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(newUser);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteTrackCommand(track.Id);
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Tracks/DeleteTrack") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task DeleteTrackEndpoint_Should_ReturnNotFound_OnNonexistentTrackId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);
        
        var track = Track.Create("TrackName",
            "AudioLink",
            new TimeSpan(0, 0, 12),
            _release.Id,
            _genre.Id);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteTrackCommand(Guid.NewGuid());
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Tracks/DeleteTrack") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}