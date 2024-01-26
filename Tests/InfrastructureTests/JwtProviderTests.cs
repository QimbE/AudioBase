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
    private readonly JwtSettings _jwtSettings = new JwtSettings
    {
        Issuer = "huh",
        Audience = "bimbimbambam",
        Key = "hehehehuhhehehehehehehehuhhwhwhwhhh",
        ExpiryTime = TimeSpan.FromMinutes(30)
    };
    
    private readonly User _defaultUser = User.Create(
        "Test",
        "test@test.ru",
        "123123",
        Role.List.First()
    );
    
    [Fact]
    public void GenerateRefreshToken_ShouldReturn_ValidToken()
    {
        // Arrange
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(_jwtSettings);

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
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(_jwtSettings);

        var provider = new JwtProvider(configMock);
        
        // Act
        var res = provider.GenerateAccessToken(_defaultUser).GetAwaiter().GetResult();
        
        // Assert
        res.Should().NotBeNull();
    }

    [Fact]
    public void GenerateVerificationToken_ShouldReturn_ValidToken()
    {
        // Arrange
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(_jwtSettings);
        
        var provider = new JwtProvider(configMock);
        
        // Act
        var res = provider.GenerateVerificationToken(_defaultUser).GetAwaiter().GetResult();
        var email = provider.GetEmailFromVerificationToken(res);
        
        // Assert
        res.Should().NotBeEmpty();

        email.Should().BeEquivalentTo(_defaultUser.Email);
    }

    public static IEnumerable<object[]> GetInvalidTokens()
    {
        yield return [""];
        
        yield return [" "];
        
        var jwtSettings = new JwtSettings
        {
            Issuer = "huh123123",
            Audience = "bimbimbambam123213222",
            Key = "hehehehuhhehehehehehehehuhhwhwhwhhh",
            ExpiryTime = TimeSpan.FromMinutes(30)
        };
        
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(jwtSettings);
        
        var provider = new JwtProvider(configMock);

        yield return [provider.GenerateAccessToken(User.Create(
            "Test",
            "test@test.ru",
            "123123",
            Role.List.First()
            )
        ).GetAwaiter().GetResult()];
    }
    
    [Theory]
    [MemberData(nameof(GetInvalidTokens))]
    public void GetEmailFromVerificationToken_Should_ReturnNull_OnInvalidToken(string token)
    {
        // Arrange
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(_jwtSettings);
        
        var provider = new JwtProvider(configMock);
        
        // Act
        var email = provider.GetEmailFromVerificationToken(token);
        
        // Assert
        email.Should().BeNull();
    }
}