using MediatR;

namespace Domain.Abstractions;

public abstract record DomainEvent(Guid Id): INotification;