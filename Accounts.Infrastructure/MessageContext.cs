using System.Threading;

namespace Accounts.Infrastructure
{
    public static class MessageContext
    {
        private static readonly AsyncLocal<string?> MessageIdAsyncLocal = new();
        public static string? MessageId 
        { 
            get => MessageIdAsyncLocal.Value; 
            set => MessageIdAsyncLocal.Value = value; 
        }

        private static readonly AsyncLocal<string?> CausationIdAsyncLocal = new();
        public static string? CausationId
        {
            get => CausationIdAsyncLocal.Value;
            set => CausationIdAsyncLocal.Value = value;
        }

        private static readonly AsyncLocal<string?> CorrelationIdAsyncLocal = new();
        public static string? CorrelationId
        {
            get => CorrelationIdAsyncLocal.Value;
            set => CorrelationIdAsyncLocal.Value = value;
        }
    }
}