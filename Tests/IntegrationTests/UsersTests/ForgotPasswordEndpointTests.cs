using System.Net;
using System.Net.Http.Json;
using Application.Authentication;
using Application.Users.ForgotPassword;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.UsersTests;

public class ForgotPasswordEndpointTests: BaseIntegrationTest
{
    public ForgotPasswordEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ForgotPassword_Should_Return_NotFound_OnNonexistentUser()
    {
        // Arrange
        var request = new ForgotPasswordQuery("123123123@mail.ru");
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ForgotPassword", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ForgotPassword_Should_Return_BaseResponse_OnValidRequest()
    {
        // Arrange
        var hashProvider = Scope.ServiceProvider.GetRequiredService<IHashProvider>();

        var passwordHash = await hashProvider.HashPassword("bimbimbimBamBamBam");
        var user = User.Create("Bim", "Bombom@mail.ru", passwordHash, Role.Admin);
        
        var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new ForgotPasswordQuery("Bombom@mail.ru");
        
        // Act
        var response = await HttpClient.PatchAsJsonAsync("User/ForgotPassword", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse>();

        content.Should().NotBeNull();
    }
}