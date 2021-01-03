using System;

namespace Accounts.Infrastructure.Exceptions
{
    public sealed class DuplicateRequestException : Exception
    {
        public object RequestId { get; }

        public DuplicateRequestException(object requestId)
            : base($"Duplicate request ({requestId}).")
        {
            RequestId = requestId;
        }
    }
}