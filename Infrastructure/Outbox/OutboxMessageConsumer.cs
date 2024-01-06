using Throw;

namespace Infrastructure.Outbox;

public sealed class OutboxMessageConsumer(Guid id, string consumerName)
{
    public Guid Id { get; init; } = id;

    public string ConsumerName { get; init; } = consumerName.Throw()
        .IfNullOrWhiteSpace(x => x);
}