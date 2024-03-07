using System.Net.Http.Json;
using Application.Authentication.Login;
using Application.Authentication.Register;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ResponseHandling.Response;

namespace IntegrationTests.GraphQL;

public class EndpointTests: BaseIntegrationTest
{
    public EndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        
    }

    private const string userQuery = """
                                 query Rofls {
                                   users {
                                     totalCount
                                     pageInfo {
                                       hasNextPage
                                       hasPreviousPage
                                       startCursor
                                       endCursor
                                     }
                                     edges {
                                       cursor
                                       node {
                                         id
                                         name
                                         email
                                         isVerified
                                       }
                                     }
                                   }
                                 }
                                 """;
    private const string genreQuery = """
                                     query {
                                         genres{
                                             nodes{
                                                 id
                                                 name
                                             }
                                         }
                                     }
                                     """;

    private const string artistQuery = """
                                       query {
                                           artists{
                                               nodes{
                                                   id
                                                   name
                                                   description
                                                   photoLink
                                               }
                                           }
                                       }
                                       """;
    
    [Fact]
    public async Task UserEndpoint_Should_ReturnUnauthorized_OnNotAdmin()
    {
        // Act
        var res = await HttpClient.GetAsync($"graphql?query={userQuery}");
        
        // Assert
        var content = await res.Content.ReadAsStringAsync();

        content.Should().StartWithEquivalentOf("""{"errors":[{"message":"The current user is not authorized to access this resource.",""");
    }

    [Fact]
    public async Task UserEndpoint_Should_ReturnValidResponse_OnValidRequest()
    {
      // Arrange
      var name = "bimbimbam";
      var email = "bam123bim@bam.ru";
      var password = "bimbimbimBamBamBam";
      
      var registerRequest = new RegisterCommand(name, email, password);
      
      var loginRequest = new LoginCommand(email, password);
      
      await HttpClient.PostAsJsonAsync("Authentication/Register", registerRequest);

      using var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

      var user = context.Users.SingleOrDefault(u => u.Email == registerRequest.Email);
      
      user!.VerifyEmail();

      user!.Update(user.Name, user.Email, user.Password, Role.Admin);

      await context.SaveChangesAsync();
      

      var loginRes = await HttpClient.PutAsJsonAsync("Authentication/Login", loginRequest);
      
      var responseWrapper = await loginRes.Content.ReadFromJsonAsync<ResponseWithData<UserResponseDto>>();
      
      HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {responseWrapper!.Data.AccessToken}"]);
      
      // Act
      var res = await HttpClient.GetAsync($"graphql?query={userQuery}");
      
      // Assert
      var content = await res.Content.ReadAsStringAsync();

      content.Should().StartWithEquivalentOf("""{"data":{"users":{"totalCount":1""");
    }
    
    [Fact]
    public async Task GenreEndpoint_Should_ReturnValidResponse_OnValidRequest()
    {
        // Arrange
        var name = "bimbimbam";
        var email = "bam123bim@bam.ru";
        var password = "bimbimbimBamBamBam";
      
        var registerRequest = new RegisterCommand(name, email, password);
      
        var loginRequest = new LoginCommand(email, password);
      
        await HttpClient.PostAsJsonAsync("Authentication/Register", registerRequest);

        using var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = context.Users.SingleOrDefault(u => u.Email == registerRequest.Email);
      
        user!.VerifyEmail();
        
        user!.Update(user.Name, user.Email, user.Password, Role.DefaultUser);

        await context.SaveChangesAsync();

        var loginRes = await HttpClient.PutAsJsonAsync("Authentication/Login", loginRequest);
      
        var responseWrapper = await loginRes.Content.ReadFromJsonAsync<ResponseWithData<UserResponseDto>>();
      
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {responseWrapper!.Data.AccessToken}"]);
        
        // Act
        var res = await HttpClient.GetAsync($"graphql?query={genreQuery}");
        
        // Assert
        var content = await res.Content.ReadAsStringAsync();

        content.Should().StartWithEquivalentOf("""{"data":{"genres":{"nodes":""");
    }
    
    [Fact]
    public async Task ArtistEndpoint_Should_ReturnValidResponse_OnValidRequest()
    {
        // Arrange
        var name = "bimbimbam";
        var email = "bam123bim@bam.ru";
        var password = "bimbimbimBamBamBam";
      
        var registerRequest = new RegisterCommand(name, email, password);
      
        var loginRequest = new LoginCommand(email, password);
      
        await HttpClient.PostAsJsonAsync("Authentication/Register", registerRequest);

        using var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = context.Users.SingleOrDefault(u => u.Email == registerRequest.Email);
      
        user!.VerifyEmail();
        
        user!.Update(user.Name, user.Email, user.Password, Role.DefaultUser);

        await context.SaveChangesAsync();

        var loginRes = await HttpClient.PutAsJsonAsync("Authentication/Login", loginRequest);
      
        var responseWrapper = await loginRes.Content.ReadFromJsonAsync<ResponseWithData<UserResponseDto>>();
      
        HttpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {responseWrapper!.Data.AccessToken}"]);
        
        // Act
        var res = await HttpClient.GetAsync($"graphql?query={artistQuery}");
        
        // Assert
        var content = await res.Content.ReadAsStringAsync();

        content.Should().StartWithEquivalentOf("""{"data":{"artists":{"nodes":""");
    }
}