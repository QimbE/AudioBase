using Domain.Abstractions;
using MediatR;

namespace Application;

// TODO: Delete this file, when the first notification handler apear
/// <summary>
/// Mocked, because of structor exceptions while decorating handlers.
/// </summary>
public class MockNotificationHandler: INotificationHandler<DomainEvent>
{
    public Task Handle(DomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}