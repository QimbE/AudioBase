﻿using Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace Infrastructure.Outbox;

/// <summary>
/// Inserts domain events of related entities to database just before actual savechanges occured;
/// </summary>
public sealed class InsertOutboxMessageInterceptor: SaveChangesInterceptor
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
        )
    {
        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }
        
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void InsertOutboxMessages(DbContext context)
    {
        var utcNow = DateTime.UtcNow;

        var outboxMessages = context.ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany<Entity, DomainEvent>(entity =>
            {
                List<DomainEvent> domainEvents = [..entity.DomainEvents];
                
                entity.ClearEventList();
                
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                domainEvent.Id,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, SerializerSettings),
                utcNow)
            )
            .ToList();
        
        context.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}