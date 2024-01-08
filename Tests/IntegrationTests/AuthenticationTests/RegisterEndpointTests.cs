using System.Net;
using System.Net.Http.Json;
using Application.Authentication.Register;
using FluentAssertions;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.AuthenticationTests;

public class RegisterEndpointTests: BaseIntegrationTest
{
    public RegisterEndpointTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        
    }

    [Theory]
    [InlineData("Boban123", "123@123.ru", "12345678")]
    [InlineData(" ", "123@123.ru", "12345678")]
    [InlineData("Boban123", "@123.", "12345678")]
    [InlineData("Boban123", "123@123.ru", "123")]
    public async Task RegisterEndpoint_ShouldReturn_ResponseDependsOnValidationResult(
        string name,
        string email,
        string password
        )
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        var request = new RegisterCommand(name, email, password);

        var validationResult = await new RegisterCommandValidator().ValidateAsync(request);
        
        // Act
        var response = await httpClient.PostAsJsonAsync("Authentication/Register", request);
        
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
        
        content.Username.Should().BeEquivalentTo(request.Name);
        content.AccessToken.Should().NotBeNullOrWhiteSpace();
        content.UserId.Should().NotBeEmpty();
        
    }
    
    [Fact]
    public async Task RegisterEndpoint_ShouldReturn_ConflictResponseOnDuplicateRegistration()
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        var request = new RegisterCommand("Boban123", "123@123.ru", "12345678");
        
        // Act
        await httpClient.PostAsJsonAsync("Authentication/Register", request);
        var response = await httpClient.PostAsJsonAsync("Authentication/Register", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}