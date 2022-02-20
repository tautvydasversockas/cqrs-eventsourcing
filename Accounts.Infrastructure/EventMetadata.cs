namespace Accounts.Infrastructure;

public sealed record EventMetadata(
    [property: JsonPropertyName("$causationId")] Guid? CausationId,
    [property: JsonPropertyName("$correlationId")] Guid? CorrelationId);
