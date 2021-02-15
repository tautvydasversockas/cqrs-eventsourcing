using CSharpFunctionalExtensions;
using System;

namespace Accounts.Domain
{
    public sealed class InterestRate : SimpleValueObject<decimal>
    {
        public InterestRate(decimal value)
            : base(value)
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(value), "Interest rate must be between 0 and 1.");
        }
    }
}