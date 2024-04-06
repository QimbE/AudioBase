using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Releases.DeleteRelease;
using Domain.Artists;
using Domain.MusicReleases;
using Domain.Tracks;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.ReleasesTests;

public class DeleteReleaseEndpointTests: BaseIntegrationTest
{
    public DeleteReleaseEndpointTests(IntegrationTestWebAppFactory factory)
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
    public async Task DeleteReleaseEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteReleaseCommand(_release.Id);
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Releases/DeleteRelease") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
        
        var tryToFind = context.Releases.SingleOrDefaultAsync(r => r.Name == _release.Name).Result;

        tryToFind.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteReleaseEndpoint_Should_ReturnBadRequest_OnNullId()
    {
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteReleaseCommand(new Guid());
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Releases/DeleteRelease") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var tryToFind = context.Releases.SingleOrDefaultAsync(r => r.Name == _release.Name).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteReleaseEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        var newUser = User.Create("Name", "email@mail.com", "password123", Role.DefaultUser);
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Users.Add(newUser);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(newUser);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteReleaseCommand(_release.Id);
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Releases/DeleteRelease") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        var tryToFind = context.Releases.SingleOrDefaultAsync(r => r.Name == _release.Name).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteReleaseEndpoint_Should_ReturnNotFound_OnNonexistentRelease()
    {
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Users.Add(_user);

        context.Artists.Add(_author);

        context.Genres.Add(_genre);

        context.Releases.Add(_release);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(_user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteReleaseCommand(Guid.NewGuid());
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Releases/DeleteRelease") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var tryToFind = context.Releases.SingleOrDefaultAsync(r => r.Name == _release.Name).Result;

        tryToFind.Should().NotBeNull();
    }
}