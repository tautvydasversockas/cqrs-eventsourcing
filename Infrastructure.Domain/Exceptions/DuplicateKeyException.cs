using System;

namespace Infrastructure.Domain.Exceptions
{
    public sealed class DuplicateKeyException : Exception
    {
        public object Key { get; }

        public DuplicateKeyException(object key)
            : base($"Duplicate key {key}.")
        {
            Key = key;
        }
    }
}