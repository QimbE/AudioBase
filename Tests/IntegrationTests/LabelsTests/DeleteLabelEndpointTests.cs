using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Labels.DeleteLabel;
using Domain.Labels;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.LabelsTests;

public class DeleteLabelEndpointTests: BaseIntegrationTest
{
    public DeleteLabelEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task DeleteLabelEndpoint_Should_ReturnBadRequest_OnNullId()
    {
        // Arrange
        var createName = "Interscope";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Labels.Add(Label.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteLabelCommand(new Guid());
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Labels/DeleteLabel") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var tryToFind = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteLabelEndpoint_Should_ReturnForbidden_OnLowUserRole()
    {
        // Arrange
        var createName = "Interscope";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.DefaultUser);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Labels.Add(Label.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteLabelCommand(labelByName.Id);
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Labels/DeleteLabel") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        var tryToFind = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteLabelEndpoint_Should_ReturnNotFound_OnNonexistentLabel()
    {
        // Arrange
        var createName = "Interscope";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Labels.Add(Label.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteLabelCommand(Guid.NewGuid());
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Labels/DeleteLabel") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var tryToFind = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;

        tryToFind.Should().NotBeNull();
    }
    
    [Fact]
    public async Task DeleteLabelEndpoint_Should_ReturnBaseResponse_OnValidRequest()
    {
        // Arrange
        var createName = "Interscope";
        
        var jwtProvider = Scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = User.Create("Bimba", "Bombom@gmail.com", "BimBam123", Role.CatalogAdmin);

        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Users.Add(user);

        context.Labels.Add(Label.Create(createName, "rofliks",
            "https://upload.wikimedia.org/wikipedia/commons/6/64/MF_Doom_-_Hultsfred_2011_%28cropped%29.jpg"));

        await context.SaveChangesAsync();

        var labelByName = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;
        
        var accessToken = await jwtProvider.GenerateAccessToken(user);
        
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {accessToken}"]);
        
        var request = new DeleteLabelCommand(labelByName.Id);
        
        // Act
        var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "Labels/DeleteLabel") { Content = JsonContent.Create(request) });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
        
        var tryToFind = context.Labels.SingleOrDefaultAsync(l => l.Name == createName).Result;

        tryToFind.Should().BeNull();
    }
}