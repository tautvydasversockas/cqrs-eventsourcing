using System;

namespace Accounts.Infrastructure.Exceptions
{
    public sealed class DuplicateOperationException : Exception
    {
        public object OperationId { get; }

        public DuplicateOperationException(object operationId)
            : base($"Duplicate operation {operationId}.")
        {
            OperationId = operationId;
        }
    }
}