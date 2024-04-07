using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Tracks.UpdateTrack;
using Domain.Artists;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.TracksTests;

public class UpdateTrackEndpointTests: BaseIntegrationTest
{
    public UpdateTrackEndpointTests(IntegrationTestWebAppFactory factory)
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
    public async Task CreateTrackEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnBadRequest_OnNullTrackId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            new Guid(),
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("0123456789 0123456789 0123456789 0123456789 0123456789 0123456789")] // Exceeds max size
    public async Task CreateTrackEndpoint_Should_ReturnBadRequest_OnInvalidName(string name)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            name, 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateTrackEndpoint_Should_ReturnBadRequest_OnInvalidLink(string link)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "TrackName", 
            link, 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData("00:01:2")]
    [InlineData("00:1:22")]
    [InlineData("1:01:22")]
    [InlineData("00:01:102")]
    [InlineData("00:102:102")]
    [InlineData("000:01:10")]
    public async Task CreateTrackEndpoint_Should_ReturnBadRequest_OnInvalidDuration(string duration)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "TrackName", 
            "TrackLink", 
            duration, 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnBadRequest_OnNullReleaseId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            new Guid(), 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnBadRequest_OnNullGenreId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            new Guid());
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var newUser = User.Create("Name", "email@mail.com", "password123", Role.DefaultUser);
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(newUser);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnNotFound_OnNonexistentTrack()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            Guid.NewGuid(), 
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnNotFound_OnNonexistentRelease()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            Guid.NewGuid(), 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnNotFound_OnNonexistentGenre()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var track = Track.Create("TrackName", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(track);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            track.Id,
            "NewTrackName", 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            Guid.NewGuid());
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnConflict_OnDuplicateName()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var (_release, _author) = GetRelease;

        var firstTrack = Track.Create("First", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        var secondTrack = Track.Create("Second", "AudioLink", 
            new TimeSpan(0, 0, 12), 
            _release.Id, _genre.Id);
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Tracks.Add(firstTrack);
        
        context.Tracks.Add(secondTrack);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateTrackCommand(
            firstTrack.Id,
            secondTrack.Name, 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Tracks/UpdateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}