using Application.DataAccess;
using Application.GraphQL;
using FluentAssertions;
using HotChocolate.Execution;
using HotChocolate.Language;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ApplicationTests;

public class CachingMiddlewareTests
{
    [Fact]
    public void InvokeAsync_ShouldCacheResponse_IfItIsNotInCache()
    {
        // Arrange
        var document = new DocumentNode([new EnumTypeDefinitionNode(
            null,
            new("bim"),
            new("bam"),
            [],
            []
            )]);
        
        Dictionary<string, object?>? toCache = new([new(
            "bimbimbim",
            new { Data = "bambambam" }
            )]);
        
        // Context mock
        var contextMock = Substitute.For<IRequestContext>();
        contextMock.Document.Returns(document);
        contextMock.Result.Returns(new QueryResult(toCache));
        
        RequestDelegate nextMock = context =>
        {
            return ValueTask.CompletedTask;
        };
        
        // CacheService mock
        var cacheMock = Substitute.For<ICacheService>();
        cacheMock.GetDataAsync<Dictionary<string, object?>?>(document.ToString()).ReturnsNull();

        var middleware = new CachingMiddleware(nextMock);
        
        // Act
        middleware.InvokeAsync(contextMock, cacheMock).GetAwaiter().GetResult();
        
        // Assert
        cacheMock.Received().SetDataAsync(document.ToString(), toCache);
    }
    
    [Fact]
    public void InvokeAsync_ShouldGetCachedValue_IfItIsInCache()
    {
        // Arrange
        var document = new DocumentNode([new EnumTypeDefinitionNode(
            null,
            new("bim"),
            new("bam"),
            [],
            []
        )]);
        
        Dictionary<string, object?>? toReturn = new([new(
            "bimbimbim",
            new { Data = "bambambam" }
        )]);
        
        // Context mock
        var contextMock = Substitute.For<IRequestContext>();
        contextMock.Document.Returns(document);
        
        RequestDelegate nextMock = context =>
        {
            return ValueTask.CompletedTask;
        };
        
        // CacheService mock
        var cacheMock = Substitute.For<ICacheService>();
        cacheMock.GetDataAsync<Dictionary<string, object?>?>(document.ToString()).Returns(toReturn);

        var middleware = new CachingMiddleware(nextMock);
        
        // Act
        middleware.InvokeAsync(contextMock, cacheMock).GetAwaiter().GetResult();
        
        // Assert
        cacheMock.Received().GetDataAsync<Dictionary<string, object?>?>(document.ToString());

        ((QueryResult)contextMock.Result).Data.Should().BeSameAs(toReturn);
    }
}