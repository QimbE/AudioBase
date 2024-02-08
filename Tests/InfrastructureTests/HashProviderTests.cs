using FluentAssertions;
using Infrastructure.Authentication;

namespace InfrastructureTests;

public class HashProviderTests
{
    [Fact]
    public async Task VerifyPassword_Should_ReturnTrue_OnSamePassword()
    {
        // Arrange
        var provider = new HashProvider();
        var password = "bimbimbim123";
        string hash;
        
        // Act
        hash = await provider.HashPassword(password);
        bool res = await provider.VerifyPassword(password, hash);
        
        // Assert
        res.Should().BeTrue();
        hash.Should().NotBeEquivalentTo(password);
    }
}