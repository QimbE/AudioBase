using Domain.Abstractions;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureTests;

/// <summary>
/// Context for tests
/// </summary>
public class TestDbContext : ApplicationDbContext
{
    public DbSet<EntityStub> Stubs { get; set; }
    
    public TestDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
}

/// <summary>
/// Stub event for tests
/// </summary>
public record StubEvent(): DomainEvent(Guid.NewGuid());

/// <summary>
/// Stub entity for tests
/// </summary>
public class EntityStub : Entity<EntityStub>
{
    public string Test { get; protected set; }
    
    private EntityStub()
    {
        
    }

    protected EntityStub(string test)
        : base(Guid.NewGuid())
    {
        Test = test;
    }

    public static EntityStub Create(string test, bool raise = true)
    {
        var stub = new EntityStub(test);
        if (raise)
        {
            stub.RaiseEvent(new StubEvent());
        }
        return stub;
    }

    public static EntityStub CreateWithManyEvents(string test)
    {
        var stub = new EntityStub(test);
        
        for (int i = 0; i < 10; i++)
        {
            stub.RaiseEvent(new StubEvent());
        }
        
        return stub;
    }
}

public class InsertOutboxMessageInterceptorTests
{
    private static InsertOutboxMessageInterceptor _interceptor = new();

    private readonly TestDbContext _context;
    
    public InsertOutboxMessageInterceptorTests()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder
            .AddInterceptors(_interceptor)
            .UseInMemoryDatabase("Test");
        
        _context = new TestDbContext(builder.Options);
    }

    public static IEnumerable<object[]> Stubs => [
        [EntityStub.Create("test1")],
        [EntityStub.Create("test2", false)],
        [EntityStub.CreateWithManyEvents("test3")]
    ];
    
    [Theory]
    [MemberData(nameof(Stubs))]
    public void SaveChangesAsync_Should_AddOutboxMessagesToDb(EntityStub stub)
    {
        // Arrange
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        var eventCount = stub.DomainEvents.Count;
        
        // Act
        _context.Stubs.Add(stub);
        _context.SaveChangesAsync().GetAwaiter().GetResult();
        var stubs = _context.Stubs.AsEnumerable();
        var outboxMessages = _context.OutboxMessages.AsEnumerable();
        
        // Assert
        outboxMessages.Count().Should().Be(eventCount);
        stubs.Count().Should().Be(1);
    }

    ~InsertOutboxMessageInterceptorTests()
    {
        _context.Dispose();
    }
}