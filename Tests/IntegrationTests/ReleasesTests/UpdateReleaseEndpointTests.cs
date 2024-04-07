using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Releases.UpdateRelease;
using Domain.Artists;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.ReleasesTests;

public class UpdateReleaseEndpointTests: BaseIntegrationTest
{
    public UpdateReleaseEndpointTests(IntegrationTestWebAppFactory factory)
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
        "Release",
        "Desc",
        "https://photo.link");
    
    private static readonly Genre _genre = Genre.Create("Rap");

    private static readonly Release _release = Release.Create(
        "Release",
        "link",
        _author.Id,
        1,
        new DateOnly(2011, 12, 12));
    
    [Fact]
    public async Task UpdateReleaseEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            "NewRelease",
            "link",
            "2012-12-12",
            _author.Id,
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
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
    public async Task UpdateReleaseEndpoint_Should_ReturnBadRequest_OnInvalidName(string name)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            name,
            "link",
            "2012-12-12",
            _author.Id,
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateReleaseEndpoint_Should_ReturnBadRequest_OnInvalidLink(string link)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            "NewRelease",
            link,
            "2012-12-12",
            _author.Id,
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData("2011-12-121")]
    [InlineData("2011-121-12")]
    [InlineData("20111-12-12")]
    [InlineData("2011-1-12")]
    [InlineData("2011-12-1")]
    public async Task UpdateReleaseEndpoint_Should_ReturnBadRequest_OnInvalidReleaseDate(string date)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            "NewRelease",
            "link",
            date,
            _author.Id,
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateReleaseEndpoint_Should_ReturnBadRequest_OnNullAuthorId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            "NewRelease",
            "link",
            "2012-12-12",
            new Guid(),
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateReleaseEndpoint_Should_ReturnBadRequest_OnNonexistentAuthorId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            "NewRelease",
            "link",
            "2012-12-12",
            Guid.NewGuid(), 
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateReleaseEndpoint_Should_ReturnNotFound_OnNonexistentReleaseTypeId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            "NewRelease",
            "link",
            "2012-12-12",
            _author.Id, 
            10000);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateReleaseEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var newUser = User.Create("Name", "email@mail.com", "password123", Role.DefaultUser);

        context.Users.Add(newUser);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(newUser);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id,
            "NewRelease",
            "link",
            "2012-12-12",
            _author.Id,
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task UpdateReleaseEndpoint_Should_ReturnNotFound_OnNonexistentRelease()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            Guid.NewGuid(), 
            "NewRelease",
            "link",
            "2012-12-12",
            _author.Id,
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateReleaseEndpoint_Should_ReturnConflict_OnDuplicateName()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        context.Releases.Add(Release.Create(
            "NewName", 
            _release.CoverLink, 
            _release.AuthorId, 
            _release.ReleaseTypeId,
            _release.ReleaseDate));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateReleaseCommand(
            _release.Id, 
            "NewName",
            "link",
            "2012-12-12",
            _author.Id,
            1);
            
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Releases/UpdateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}