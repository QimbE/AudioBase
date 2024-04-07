using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Tracks.CreateTrack;
using Domain.Artists;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.TracksTests;

public class CreateTrackEndpointTests: BaseIntegrationTest
{
    public CreateTrackEndpointTests(IntegrationTestWebAppFactory factory)
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
        
        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateTrackCommand(
            "TrackName", 
            "AudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
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

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateTrackCommand(
            name, 
            "AudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateTrackEndpoint_Should_ReturnBadRequest_OnInvalidAudioLink(string link)
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
        
        var request = new CreateTrackCommand(
            "TrackName", 
            link, 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
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

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateTrackCommand(
            "TrackName", 
            "AudioLink", 
            duration, 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
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

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateTrackCommand(
            "TrackName", 
            "AudioLink", 
            "00:12:01", 
            new Guid(), 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
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

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateTrackCommand(
            "TrackName", 
            "AudioLink", 
            "00:12:01", 
            _release.Id, 
            new Guid());
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var newUser = User.Create("Name", "email@mail.com", "password123", Role.DefaultUser);

        var (_release, _author) = GetRelease;

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(newUser);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateTrackCommand(
            "TrackName", 
            "AudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnNotFound_OnNonexistentReleaseId()
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
        
        var request = new CreateTrackCommand(
            "TrackName", 
            "AudioLink", 
            "00:02:54", 
            Guid.NewGuid(), 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateTrackEndpoint_Should_ReturnNotFound_OnNonexistentGenreId()
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
        
        var request = new CreateTrackCommand(
            "TrackName", 
            "AudioLink", 
            "00:02:54", 
            _release.Id, 
            Guid.NewGuid());
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
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
        
        var request = new CreateTrackCommand(
            track.Name, 
            "NewAudioLink", 
            "00:02:54", 
            _release.Id, 
            _genre.Id);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Tracks/CreateTrack", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}