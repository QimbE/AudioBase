using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Labels.UpdateLabel;
using Domain.Labels;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.LabelsTests;

public class UpdateLabelEndpointTests: BaseIntegrationTest
{
    public UpdateLabelEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task UpdateLabelEndpoint_Should_ReturnBadRequest_OnNullId()
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

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(new Guid(), newName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("0123456789 0123456789 0123456789 0123456789 0123456789 0123456789")] // Exceeds max size
    public async Task UpdateLabelEndpoint_Should_ReturnBadRequest_OnInvalidLabelName(string labelName)
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l=> l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(labelByName.Id, labelName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateLabelEndpoint_Should_ReturnBadRequest_OnInvalidDescription()
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

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l=> l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(labelByName.Id, createName, tooLarge, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateLabelEndpoint_Should_ReturnBadRequest_OnInvalidPhotoLink(string photoLink)
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l=> l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(labelByName.Id, createName, createDesc, photoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateLabelEndpoint_Should_ReturnForbidden_OnLowUserRole()
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

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l=> l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(labelByName.Id, newName, createDesc, newPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task UpdateLabelEndpoint_Should_ReturnNotFound_OnNonexistentLabel()
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

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(Guid.NewGuid(), newName, createDesc, newPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateLabelEndpoint_Should_ReturnConflict_OnEqualData()
    {
        // Arrange
        string createName = "OldName";
        string createDesc = "Desc";
        string createPhotoLink = "https://rofliks.com/roflan.gif";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l=> l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(labelByName.Id, createName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task UpdateLabelEndpoint_Should_ReturnConflict_OnDuplicateName()
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

        context.Labels.Add(Label.Create(toChangeName, createDesc, createPhotoLink));
        context.Labels.Add(Label.Create(toFindName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l=> l.Name == toChangeName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(labelByName.Id, toFindName, createDesc, createPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task UpdateLabelEndpoint_Should_ReturnBaseResponse_OnValidRequest()
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

        context.Labels.Add(Label.Create(createName, createDesc, createPhotoLink));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l=> l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new UpdateLabelCommand(labelByName.Id, newName, createDesc, newPhotoLink);
        
        // Act
        var response = await HttpClient.PutAsJsonAsync("Labels/UpdateLabel", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}