using System.Net;
using System.Net.Http.Json;
using Application.Authentication.Login;
using Application.Authentication.Register;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.AuthenticationTests;

public class LoginEndpointTests: BaseIntegrationTest
{
    public LoginEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        
    }
    
    [Theory]
    [InlineData("Boban123", "123@123.ru", "12345678")]
    [InlineData("Boban123", "@123.", "12345678")]
    [InlineData("Boban123", "123@123.ru", "123")]
    public async Task LoginEndpoint_ShouldReturn_ResponseDependsOnValidationResult(
        string name,
        string email,
        string password
    )
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        var loginRequest = new LoginCommand(email, password);

        var registerRequest = new RegisterCommand(name, email, password);

        var validationResult = await new LoginCommandValidator().ValidateAsync(loginRequest);
        
        // Act
        await httpClient.PostAsJsonAsync("Authentication/Register", registerRequest);
        
        if (validationResult.IsValid)
        {
            using var scope = Factory.Services.CreateScope();
            
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == registerRequest.Email);
            
            user!.VerifyEmail();

            await context.SaveChangesAsync();
        }
        
        var response = await httpClient.PutAsJsonAsync("Authentication/Login", loginRequest);
        
        // Assert
        if (!validationResult.IsValid)
        {
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            return;
        }
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        IEnumerable<string>? cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

        cookies.Single().Should().StartWith("refreshToken");
        
        var responseWrapper = await response.Content.ReadFromJsonAsync<ResponseWithData<UserResponseDto>>();
        
        responseWrapper.Should().NotBeNull();

        var content = responseWrapper.Data;
        
        content.AccessToken.Should().NotBeNullOrWhiteSpace();
        content.UserId.Should().NotBeEmpty();
    }
    
    [Theory]
    [InlineData("Expected@mail.ru", "Hehehehuh123", "Expected@mail.ru", "Hehehehuh1")]
    [InlineData("Expected@mail.ru", "Hehehehuh123", "Expected1@mail.ru", "Hehehehuh123")]
    public async Task LoginEndpoint_ShouldReturn_BadRequestOnInvalidCredentials(
        string expectedEmail,
        string expectedPassword,
        string actualEmail,
        string actualPassword
        )
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        var loginRequest = new LoginCommand(actualEmail, actualPassword);

        var registerRequest = new RegisterCommand("SomeName123123", expectedEmail, expectedPassword);
        
        // Act
        await httpClient.PostAsJsonAsync("Authentication/Register", registerRequest);
        var response = await httpClient.PutAsJsonAsync("Authentication/Login", loginRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}