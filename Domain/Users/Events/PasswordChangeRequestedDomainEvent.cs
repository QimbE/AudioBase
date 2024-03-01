using Domain.Abstractions;

namespace Domain.Users.Events;

public record PasswordChangeRequestedDomainEvent(Guid UserId): DomainEvent(Guid.NewGuid());