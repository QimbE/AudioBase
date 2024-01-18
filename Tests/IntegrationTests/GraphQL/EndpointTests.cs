﻿using System.Net.Http.Json;
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

    private const string Query = """
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
    
    [Fact]
    public async Task UserEndpoint_Should_ReturnUnauthorized_OnNotAdmin()
    {
        // Arrange
        var httpClient = Factory.CreateClient();

        // Act
        var res = await httpClient.GetAsync($"graphql?query={Query}");
        
        // Assert
        var content = await res.Content.ReadAsStringAsync();

        content.Should().StartWithEquivalentOf("""{"errors":[{"message":"The current user is not authorized to access this resource.",""");
    }

    [Fact]
    public async Task UserEndpoint_Should_ReturnValidResponse_OnValidRequest()
    {
      // Arrange
      var httpClient = Factory.CreateClient();

      var name = "bimbimbam";
      var email = "bam123bim@bam.ru";
      var password = "bimbimbimBamBamBam";
      
      var registerRequest = new RegisterCommand(name, email, password);
      
      var loginRequest = new LoginCommand(email, password);
      
      await httpClient.PostAsJsonAsync("Authentication/Register", registerRequest);
      

      using var scope = Factory.Services.CreateScope();

      using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

      var user = context.Users.SingleOrDefault(u => u.Email == registerRequest.Email);
      
      user!.VerifyEmail();

      user!.Update(user.Name, user.Email, user.Password, Role.Admin);

      await context.SaveChangesAsync();
      

      var loginRes = await httpClient.PutAsJsonAsync("Authentication/Login", loginRequest);
      
      var responseWrapper = await loginRes.Content.ReadFromJsonAsync<ResponseWithData<UserResponseDto>>();
      
      httpClient.DefaultRequestHeaders.Add("Authorization", [$"Bearer {responseWrapper!.Data.AccessToken}"]);
      
      // Act
      var res = await httpClient.GetAsync($"graphql?query={Query}");
      
      // Assert
      var content = await res.Content.ReadAsStringAsync();

      content.Should().StartWithEquivalentOf("""{"data":{"users":{"totalCount":1""");
    }
}