using System;
using System.Threading;

namespace Accounts.Infrastructure
{
    public static class MessageContext
    {
        private static readonly AsyncLocal<Guid?> MessageIdAsyncLocal = new();
        public static Guid? MessageId 
        { 
            get => MessageIdAsyncLocal.Value; 
            set => MessageIdAsyncLocal.Value = value; 
        }

        private static readonly AsyncLocal<Guid?> CausationIdAsyncLocal = new();
        public static Guid? CausationId
        {
            get => CausationIdAsyncLocal.Value;
            set => CausationIdAsyncLocal.Value = value;
        }

        private static readonly AsyncLocal<Guid?> CorrelationIdAsyncLocal = new();
        public static Guid? CorrelationId
        {
            get => CorrelationIdAsyncLocal.Value;
            set => CorrelationIdAsyncLocal.Value = value;
        }
    }
}