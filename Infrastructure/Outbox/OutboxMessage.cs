namespace Infrastructure.Outbox;

/// <summary>
/// A part of implementation of transactional outbox pattern.
/// </summary>
/// <param name="id">Id of this message</param>
/// <param name="name">Name of event, that produced this message</param>
/// <param name="content">Json with actual event to handle</param>
/// <param name="createdOnUtc">Time when was created</param>
/// <param name="processedOnUtc">Time when was processed</param>
/// <param name="error">Some error while handling (if occured)</param>
public sealed class OutboxMessage(
    Guid id,
    string name,
    string content,
    DateTime createdOnUtc,
    DateTime? processedOnUtc = null,
    string? error = null
)
{
    public Guid Id { get; init; } = id;
    
    public string Name { get; init; } = name;
    
    public string Content { get; init; } = content;
    
    public DateTime CreatedOnUtc { get; init; } = createdOnUtc;
    
    public DateTime? ProcessedOnUtc { get; set; } = processedOnUtc;

    public string? Error { get; set; } = error;
}