﻿using System;
using System.Collections.Generic;
using Infrastructure.Domain;

namespace Accounts.Domain
{
    public sealed class InterestRate : ValueObject
    {
        private readonly decimal _value;

        public InterestRate(decimal value)
        {
            if (value < 0 || value > 1)
                throw new ArgumentException("Interest rate must be between 0 and 1.");

            _value = value;
        }

        public static implicit operator decimal(InterestRate interestRate) => interestRate._value;
        public static explicit operator InterestRate(decimal value) => new InterestRate(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }
    }
}
