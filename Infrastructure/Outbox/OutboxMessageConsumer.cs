﻿using Throw;

namespace Infrastructure.Outbox;

public sealed class OutboxMessageConsumer(Guid id, string name)
{
    public Guid Id { get; init; } = id;

    public string ConsumerName { get; init; } = name.Throw()
        .IfNullOrWhiteSpace(x => x);
}