using CSharpFunctionalExtensions;
using System;

namespace Accounts.Domain
{
    public sealed class Money : SimpleValueObject<decimal>
    {
        public Money(decimal value)
            : base(value) 
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Monetary value must be positive.");
        }
    }
}