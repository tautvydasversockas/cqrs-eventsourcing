using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Events
{
    public sealed class DepositedToAccount : Event
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public DepositedToAccount(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}