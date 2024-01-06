using Domain.Abstractions;
using FluentAssertions;

namespace InfrastructureTests;

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

public class InsertOutboxMessageInterceptorTests: InfrastructureTestBase
{
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
        RecreateDatabase();
        var eventCount = stub.DomainEvents.Count;
        
        // Act
        Context.Stubs.Add(stub);
        Context.SaveChangesAsync().GetAwaiter().GetResult();
        var stubs = Context.Stubs.AsEnumerable();
        var outboxMessages = Context.OutboxMessages.AsEnumerable();
        
        // Assert
        outboxMessages.Count().Should().Be(eventCount);
        stubs.Count().Should().Be(1);
    }

    ~InsertOutboxMessageInterceptorTests()
    {
        Context.Dispose();
    }
}