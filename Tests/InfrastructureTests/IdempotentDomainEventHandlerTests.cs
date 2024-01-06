using FluentAssertions;
using Infrastructure.Idempotence;
using MediatR;
using NSubstitute;

namespace InfrastructureTests;

public class IdempotentDomainEventHandlerTests: InfrastructureTestBase
{
    [Fact]
    public void IdempotentHandler_ShouldNot_ExecuteActualHandler_OnRepeatedCall()
    {
        // Arrange
        RecreateDatabase();
        
        var handlerMock = Substitute.For<INotificationHandler<StubEvent>>();
        var fakeEvent = new StubEvent();
        handlerMock.Handle(fakeEvent, default).Returns(Task.CompletedTask);
        
        var idempotentHandler = new IdempotentDomainEventHandler<StubEvent>(handlerMock, Context);
        
        // Act
        for (int i = 0; i < 3; i++)
        {
            idempotentHandler.Handle(fakeEvent, default).GetAwaiter().GetResult();
        }

        // Assert
        handlerMock.Received(1).Handle(fakeEvent, default);

        var messageConsumers = Context.OutboxMessageConsumers.AsEnumerable();

        messageConsumers.Count().Should().Be(1);
    }

    [Fact]
    public void IdempotentHandler_Should_ExecuteActualHandler_OnDifferentEvents()
    {
        // Arrange
        RecreateDatabase();
        
        var handlerMock = Substitute.For<INotificationHandler<StubEvent>>();
        var fakeEvent1 = new StubEvent();
        var fakeEvent2 = new StubEvent();
        
        handlerMock.Handle(fakeEvent1, default).Returns(Task.CompletedTask);
        handlerMock.Handle(fakeEvent2, default).Returns(Task.CompletedTask);
        
        var idempotentHandler = new IdempotentDomainEventHandler<StubEvent>(handlerMock, Context);
        
        // Act
        idempotentHandler.Handle(fakeEvent1, default).GetAwaiter().GetResult();
        idempotentHandler.Handle(fakeEvent2, default).GetAwaiter().GetResult();
        

        // Assert
        handlerMock.Received(1).Handle(fakeEvent1, default);
        handlerMock.Received(1).Handle(fakeEvent2, default);

        var messageConsumers = Context.OutboxMessageConsumers.AsEnumerable();

        messageConsumers.Count().Should().Be(2);
    }
}