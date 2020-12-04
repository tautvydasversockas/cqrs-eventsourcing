using System.Text.Json.Serialization;

namespace Accounts.Infrastructure
{
    public sealed record Metadata(
        [property:JsonPropertyName("$causationId")] string? CausationId,
        [property:JsonPropertyName("$correlationId")] string? CorrelationId);
}