using Domain.Users;
using FluentAssertions;
using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace InfrastructureTests;

public class JwtProviderTests
{
    [Fact]
    public void GenerateRefreshToken_ShouldReturn_ValidToken()
    {
        // Arrange
        var configMock = Substitute.For<IConfiguration>();

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

        var sectionMock = Substitute.For<IConfigurationSection>();

        sectionMock["Key"].Returns("hehehehuhhehehehehehehehuhhwhwhwhhh");
        sectionMock["Issuer"].Returns("huh");
        sectionMock["Audience"].Returns("bimbimbambam");
        sectionMock["ExpiryTime"].Returns("00:30:00");
        
        var configMock = Substitute.For<IConfiguration>();

        configMock.GetSection("JwtSettings").Returns(sectionMock);

        var provider = new JwtProvider(configMock);
        
        // Act
        var res = provider.Generate(user).GetAwaiter().GetResult();
        
        // Assert
        res.Should().NotBeNull();
    }
}