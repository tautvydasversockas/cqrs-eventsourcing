namespace Accounts.Infrastructure
{
    public sealed class MessageContext
    {
        public string MessageId { get; }
        public string CausationId { get; }
        public string CorrelationId { get; }

        public MessageContext(string messageId, string causationId, string correlationId)
        {
            MessageId = messageId;
            CausationId = causationId;
            CorrelationId = correlationId;
        }
    }
}