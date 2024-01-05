using Domain.Abstractions;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Quartz;

namespace Infrastructure.BackgroundJobs;

// TODO: Probably we'll have to extract the whole background job service in separate project/solution
/// <summary>
/// Background job to process domain (integration?) events
/// </summary>
[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob: IJob
{
    private readonly ApplicationDbContext _context;
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    /// <summary>
    /// The maximum amount of events to handle by only one job run
    /// </summary>
    public const int MaxMessagesForOneJob = 20;

    /// <summary>
    /// The maximum retry count for publish 1 event
    /// </summary>
    public const int MaxRetryCount = 3;

    /// <summary>
    /// Delay step for retries
    /// </summary>
    public const int RetryDelayMilliseconds = 50;

    public ProcessOutboxMessagesJob(ApplicationDbContext context, IPublisher publisher, ILogger<ProcessOutboxMessagesJob> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // Unprocessed messages
        var messages = await _context.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .Take(MaxMessagesForOneJob)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            // Something tells me that it is actually Integration, not Domain event (but who cares?)
            var domainEvent = JsonConvert
                .DeserializeObject<DomainEvent>(message.Content);

            // Pretty bad scenario, because we can't even debug to know what happend
            if (domainEvent is null)
            {
                _logger.LogWarning(
                    "Outbox message {@name} with Id: {@id} turned out to contain null event.",
                    message.Name,
                    message.Id
                    );

                message.Error = "Something went wrong, while deserializing content of this message";
                message.ProcessedOnUtc = DateTime.UtcNow;
                continue;
            }

            // Retry policy configuration
            AsyncRetryPolicy policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    MaxRetryCount,
                    attempt => TimeSpan.FromMilliseconds(RetryDelayMilliseconds * attempt)
                    );

            // Executing with retry on exceptions
            PolicyResult result = await policy.ExecuteAndCaptureAsync(
                () => _publisher.Publish(domainEvent, context.CancellationToken)
                );

            // Writing an error if there are some
            message.Error = result.FinalException?.ToString();
            
            message.ProcessedOnUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(context.CancellationToken);
    }
}