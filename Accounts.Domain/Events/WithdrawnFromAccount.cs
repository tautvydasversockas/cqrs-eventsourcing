using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Events
{
    public sealed class WithdrawnFromAccount : Event
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public WithdrawnFromAccount(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}