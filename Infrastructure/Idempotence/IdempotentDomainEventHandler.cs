using Domain.Abstractions;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Idempotence;

/// <summary>
/// Decorator for event handler to enforce idempotence (google this smart word).
/// </summary>
/// <typeparam name="TDomainEvent"></typeparam>
public class IdempotentDomainEventHandler<TDomainEvent>: INotificationHandler<TDomainEvent> 
    where TDomainEvent: DomainEvent
{
    private readonly INotificationHandler<TDomainEvent> _decorated;
    private readonly ApplicationDbContext _context;

    public IdempotentDomainEventHandler(
        INotificationHandler<TDomainEvent> decorated,
        ApplicationDbContext context
        )
    {
        _decorated = decorated;
        _context = context;
    }
    
    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        string consumer = _decorated.GetType().Name;

        // If this is not the first execution
        if (await _context.OutboxMessageConsumers.AnyAsync(
                c =>
                    c.Id == notification.Id &&
                     c.ConsumerName == consumer,
                cancellationToken)
            )
        {
            return;
        }

        await _decorated.Handle(notification, cancellationToken);

        // Mark that this message was published successfully
        _context.OutboxMessageConsumers
            .Add(new (notification.Id, consumer));

        await _context.SaveChangesAsync(cancellationToken);
    }
}