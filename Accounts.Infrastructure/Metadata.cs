using Newtonsoft.Json;

namespace Accounts.Infrastructure
{
    public sealed class Metadata
    {
        [JsonProperty("$causationId")]
        public string CausationId { get; }

        [JsonProperty("$correlationId")]
        public string CorrelationId { get; }

        public Metadata(string causationId, string correlationId)
        {
            CausationId = causationId;
            CorrelationId = correlationId;
        }
    }
}