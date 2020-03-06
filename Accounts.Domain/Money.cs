using System;
using System.Collections.Generic;
using Infrastructure.Domain;

namespace Accounts.Domain
{
    public sealed class Money : ValueObject<Money>
    {
        private readonly decimal _value;

        public Money(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Money must be positive");

            _value = value;
        }

        public static implicit operator decimal(Money money) => money._value;
        public static explicit operator Money(decimal value) => new Money(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }
    }
}
