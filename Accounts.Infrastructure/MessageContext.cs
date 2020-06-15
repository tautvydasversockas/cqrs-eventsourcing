namespace Accounts.Infrastructure
{
    public sealed class MessageContext
    {
        public string Id { get; }
        public string CausationId { get; }
        public string CorrelationId { get; }

        public MessageContext(string id, string causationId, string correlationId)
        {
            Id = id;
            CausationId = causationId;
            CorrelationId = correlationId;
        }
    }
}