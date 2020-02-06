using System;
using Infrastructure.Domain;

namespace Accounts.Domain.Events
{
    public sealed class DepositedToAccount : VersionedEvent<Guid>
    {
        public decimal Amount { get; }

        public DepositedToAccount(decimal amount)
        {
            Amount = amount;
        }
    }
}