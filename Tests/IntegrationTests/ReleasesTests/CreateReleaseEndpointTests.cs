using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Releases.CreateRelease;
using Domain.Artists;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Endpoints;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.ReleasesTests;

public class CreateReleaseEndpointTests: BaseIntegrationTest
{
    public CreateReleaseEndpointTests(IntegrationTestWebAppFactory factory)
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
    
    [Fact]
    public async Task CreateReleaseEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("Name", "CoverLink", "2011-12-12", _author.Id, 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
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
    public async Task CreateReleaseEndpoint_Should_ReturnBadRequest_OnInvalidName(string name)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand(name, "CoverLink", "2011-12-12", _author.Id, 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateReleaseEndpoint_Should_ReturnBadRequest_OnInvalidCoverLink(string coverLink)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("Name", coverLink, "2011-12-12", _author.Id, 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData("2011-12-121")]
    [InlineData("2011-121-12")]
    [InlineData("20111-12-12")]
    [InlineData("2011-1-12")]
    [InlineData("2011-12-1")]
    public async Task CreateReleaseEndpoint_Should_ReturnBadRequest_OnInvalidDateFormat(string date)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("Name", "CoverLink", date, _author.Id, 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateReleaseEndpoint_Should_ReturnBadRequest_OnNullAuthorId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("Name", "CoverLink", "date", new Guid(), 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateReleaseEndpoint_Should_ReturnBadRequest_OnNonexistentAuthorId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("Name", "CoverLink", "2011-12-12", Guid.NewGuid(), 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateReleaseEndpoint_Should_ReturnBadRequest_OnInvalidTypeId()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("Name", "CoverLink", "2011-12-12", _author.Id, 10000);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateReleaseEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var newUser = User.Create("Name", "email@mail.com", "password123", Role.DefaultUser);
        
        context.Users.Add(newUser);

        context.Artists.Add(_author);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(newUser);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("Name", "CoverLink", "2011-12-12", _author.Id, 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateReleaseEndpoint_Should_ReturnConflict_OnAlreadyExistentName()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Releases.Add(Release.Create("OldName", "CoverLink", _author.Id, 1, new DateOnly(2001, 11, 11)));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateReleaseCommand("OldName", "NewCoverLink", "2011-11-11", _author.Id, 1);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Releases/CreateRelease", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}