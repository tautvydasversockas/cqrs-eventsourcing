using System;
using Infrastructure.Domain;

namespace Accounts.Domain.Events
{
    public sealed class WithdrawnFromAccount : VersionedEvent<Guid>
    {
        public decimal Amount { get; }

        public WithdrawnFromAccount(decimal amount)
        {
            Amount = amount;
        }
    }
}