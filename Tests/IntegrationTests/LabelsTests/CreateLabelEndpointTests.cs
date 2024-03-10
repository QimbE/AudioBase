using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Labels.CreateLabel;
using Domain.Labels;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.LabelsTests;

public class CreateLabelEndpointTests: BaseIntegrationTest
{
    public CreateLabelEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("0123456789 0123456789 0123456789 0123456789 0123456789 0123456789")] // Exceeds max size
    public async Task CreateLabelEndpoint_Should_ReturnBadRequest_OnInvalidLabelName(string labelName)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateLabelCommand(labelName, "rofliks", "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Labels/CreateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateLabelEndpoint_Should_ReturnBadRequest_OnInvalidLabelDescription()
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
        
        var request = new CreateLabelCommand("Interscope", tooLarge, "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Labels/CreateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateLabelEndpoint_Should_ReturnBadRequest_OnInvalidPhotoLink(string photoLink)
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateLabelCommand("MF DOOM", "rofliks", photoLink);
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Labels/CreateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateLabelEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.DefaultUser);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateLabelCommand("MF DOOM", "rofliks", "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Labels/CreateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateLabelEndpoint_Should_ReturnConflict_OnDuplicateLabelName()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var firstLabel = Label.Create("Interscope", "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");

        context.Users.Add(user);

        context.Labels.Add(firstLabel);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);

        var request = new CreateLabelCommand("Interscope", "roflanchik", "https://rofliks.com/rofl.gif");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Labels/CreateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task CreateLabelEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new CreateLabelCommand("Interscope", "rofliks", "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg");
        
        // Act
        var response = await HttpClient.PostAsJsonAsync("Labels/CreateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}