using FluentAssertions;
using Infrastructure.BackgroundJobs;
using Infrastructure.Outbox;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using Quartz;

namespace InfrastructureTests;

public class ProcessOutboxMessagesJobTests: InfrastructureTestBase
{
    public ProcessOutboxMessagesJobTests()
        : base(typeof(ProcessOutboxMessagesJobTests))
    {
        
    }
    
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    
    public static IEnumerable<object[]> ValidOutboxMessagesToHandle()
    {
        List<StubEvent> fakeEvents = [new(), new(), new(), new()];
        
        List<OutboxMessage> firstTest = [];
        
        for (int i = 0; i < fakeEvents.Count(); i++)
        {
            firstTest.Add(
                new(
                    fakeEvents[i].Id,
                    fakeEvents[i].GetType().Name,
                    JsonConvert.SerializeObject(fakeEvents[i], SerializerSettings),
                    DateTime.UtcNow
                    )
                );
        }

        yield return [firstTest];
        
        List<OutboxMessage> secondTest = [];

        for (int i = 0; i < fakeEvents.Count() / 2; i++)
        {
            secondTest.Add(
                new(
                    fakeEvents[i].Id,
                    fakeEvents[i].GetType().Name,
                    JsonConvert.SerializeObject(fakeEvents[i], SerializerSettings),
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddSeconds(50)
                )
            );
        }
        
        for (int i = fakeEvents.Count() / 2; i < fakeEvents.Count(); i++)
        {
            secondTest.Add(
                new(
                    fakeEvents[i].Id,
                    fakeEvents[i].GetType().Name,
                    JsonConvert.SerializeObject(fakeEvents[i], SerializerSettings),
                    DateTime.UtcNow
                )
            );
        }
        
        yield return [secondTest];
        
        List<OutboxMessage> thirdTest = [];

        yield return [thirdTest];

        List<OutboxMessage> fourthTest = [];
        
        for (int i = 0; i < fakeEvents.Count() / 2; i++)
        {
            fourthTest.Add(
                new(
                    fakeEvents[i].Id,
                    fakeEvents[i].GetType().Name,
                    JsonConvert.SerializeObject(fakeEvents[i], SerializerSettings),
                    DateTime.UtcNow
                )
            );
        }
        
        for (int i = fakeEvents.Count() / 2; i < fakeEvents.Count(); i++)
        {
            fourthTest.Add(
                new(
                    fakeEvents[i].Id,
                    fakeEvents[i].GetType().Name,
                    " ",
                    DateTime.UtcNow
                )
            );
        }

        yield return [fourthTest];
    }

    [Theory]
    [MemberData(nameof(ValidOutboxMessagesToHandle))]
    public void Job_Should_Handle_ValidMessages(IEnumerable<OutboxMessage> messageList)
    {
        // Arrange
        RecreateDatabase();
        
        Context.AddRange(messageList);
        Context.SaveChangesAsync().GetAwaiter().GetResult();

        var publisherMock = Substitute.For<IPublisher>();

        publisherMock.Publish(null).ReturnsForAnyArgs(Task.CompletedTask);
        
        var loggerMock = Substitute.For<ILogger<ProcessOutboxMessagesJob>>();
        var job = new ProcessOutboxMessagesJob(Context, publisherMock, loggerMock);

        var jobContext = Substitute.For<IJobExecutionContext>();
        
        // Act
        job.Execute(jobContext).GetAwaiter().GetResult();
        
        // Assert
        // There are no any unprocessed messages even if the content was null or invalid
        Context.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .AsEnumerable()
            .Any().Should().Be(false);
    }

    public class PublisherMock : IPublisher
    {
        // random exception just not to throw default
        public Task Publish(object notification, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new InvalidCastException();
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
        {
            throw new InvalidCastException();
        }
    }
    
    [Fact]
    public void Job_Should_HandleExceptions_OnEventPublishing()
    {
        // Arrange
        RecreateDatabase();
        
        var fakeEvent = new StubEvent();
        
        List<OutboxMessage> messageList=[
            new(
                fakeEvent.Id,
                fakeEvent.GetType().Name,
                JsonConvert.SerializeObject(fakeEvent, SerializerSettings),
                DateTime.UtcNow
                )
        ];
        
        Context.AddRange(messageList);
        Context.SaveChangesAsync().GetAwaiter().GetResult();

        var publisherMock = new PublisherMock();
        
        var loggerMock = Substitute.For<ILogger<ProcessOutboxMessagesJob>>();
        var job = new ProcessOutboxMessagesJob(Context, publisherMock, loggerMock);

        var jobContext = Substitute.For<IJobExecutionContext>();
        
        // Act
        job.Execute(jobContext).GetAwaiter().GetResult();
        
        // Assert
        // There are no any unprocessed messages even if the content was null or invalid
        Context.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .AsEnumerable()
            .Any().Should().Be(false);
    }
}