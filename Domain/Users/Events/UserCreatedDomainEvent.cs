using Domain.Abstractions;

namespace Domain.Users.Events;

public record UserCreatedDomainEvent(Guid UserId): DomainEvent(Guid.NewGuid());