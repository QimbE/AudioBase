using Domain.Users;
using FluentAssertions;
using Infrastructure.Authentication;
using Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace InfrastructureTests;

public class JwtProviderTests
{
    [Fact]
    public void GenerateRefreshToken_ShouldReturn_ValidToken()
    {
        // Arrange
        var sectionMock = new JwtSettings
        {
            Issuer = "huh",
            Audience = "bimbimbambam",
            Key = "hehehehuhhehehehehehehehuhhwhwhwhhh",
            ExpiryTime = TimeSpan.FromMinutes(30)
        };
        
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(sectionMock);

        var provider = new JwtProvider(configMock);

        // Act

        var res = provider.GenerateRefreshToken();
        
        // Assert

        res.Should().NotBeEmpty();
    }

    [Fact]
    public void Generate_ShouldReturn_ValidToken()
    {
        // Arrange
        var user = User.Create(
            "Test",
            "test@test.ru",
            "123123",
            Role.List.First()
            );

        var sectionMock = new JwtSettings
        {
            Issuer = "huh",
            Audience = "bimbimbambam",
            Key = "hehehehuhhehehehehehehehuhhwhwhwhhh",
            ExpiryTime = TimeSpan.FromMinutes(30)
        };
        
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(sectionMock);

        var provider = new JwtProvider(configMock);
        
        // Act
        var res = provider.Generate(user).GetAwaiter().GetResult();
        
        // Assert
        res.Should().NotBeNull();
    }
}