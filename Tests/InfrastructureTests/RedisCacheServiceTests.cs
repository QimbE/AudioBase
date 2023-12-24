using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using Infrastructure.Cache;
using NSubstitute;
using StackExchange.Redis;
using Throw;

namespace InfrastructureTests;

public class RedisCacheServiceTests
{
    [Fact]
    public void GetDataAsync_ShouldThrowException_OnNullOrWhiteSpaceKey()
    {
        // Arrange
        var key = "      ";
        
        var cacheDb = Substitute.For<IDatabase>();
        
        var multiplexer = Substitute.For<IConnectionMultiplexer>();
        multiplexer.GetDatabase().Returns(cacheDb);
        
        var redisCache = new RedisCacheService(multiplexer);
        
        // Act
        var action = () =>
            redisCache.GetDataAsync<string>(key)
                .GetAwaiter().GetResult();
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void GetDataAsync_ShouldReturnNull_OnNonexistentKey()
    {
        // Arrange
        var key = "somekey";

        var value = RedisValue.Null;
        
        var cacheDb = Substitute.For<IDatabase>();
        cacheDb.StringGet(key).Returns(value);
        
        var multiplexer = Substitute.For<IConnectionMultiplexer>();
        multiplexer.GetDatabase().Returns(cacheDb);
        
        var redisCache = new RedisCacheService(multiplexer);
        
        // Act
        var res = redisCache.GetDataAsync<string>(key)
                .GetAwaiter().GetResult();
        
        // Assert
        res.Should().BeNull();
    }
    
    [Fact]
    public void GetDataAsync_ShouldReturnValue_OnExistentKey()
    {
        // Arrange
        var key = "somekey";

        var value = new RedisValue(@"""somevalue""");
        
        var cacheDb = Substitute.For<IDatabase>();
        cacheDb.StringGet(key).Returns(value);
        
        var multiplexer = Substitute.For<IConnectionMultiplexer>();
        multiplexer.GetDatabase().Returns(cacheDb);
        
        var redisCache = new RedisCacheService(multiplexer);
        
        // Act
        var res = redisCache.GetDataAsync<string>(key)
            .GetAwaiter().GetResult();
        
        // Assert
        res.Should().Be(((string)value).Trim('"'));
    }
    
    [Fact]
    public void SetDataAsync_ShouldThrowException_OnNullOrWhiteSpaceKey()
    {
        // Arrange
        var key = "      ";
        
        var cacheDb = Substitute.For<IDatabase>();
        
        var multiplexer = Substitute.For<IConnectionMultiplexer>();
        multiplexer.GetDatabase().Returns(cacheDb);
        
        var redisCache = new RedisCacheService(multiplexer);
        
        // Act
        var action = () =>
            redisCache.SetDataAsync(key, new {test = 1})
                .GetAwaiter().GetResult();
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void SetDataAsync_ShouldReturnTrue_OnValidKey()
    {
        // Arrange
        var key = "somekey";
        
        var cacheDb = Substitute.For<IDatabase>();
        
        var multiplexer = Substitute.For<IConnectionMultiplexer>();
        multiplexer.GetDatabase().Returns(cacheDb);
        
        var redisCache = new RedisCacheService(multiplexer);

        var expiry = TimeSpan.FromSeconds(
            (int)redisCache
                .GetType()
                .GetField("SecondsExpirationTime", BindingFlags.NonPublic | BindingFlags.Static)!
                .GetValue(null)!
            );
        
        cacheDb.StringSet(
            key,
            JsonSerializer.Serialize(new {test = 1}),
            expiry
        ).Returns(true);
        
        // Act
        var res = redisCache.SetDataAsync(key, new {test = 1})
                .GetAwaiter().GetResult();
        
        // Assert
        res.Should().BeTrue();
    }
}