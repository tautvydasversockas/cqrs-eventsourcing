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

        public override string ToString()
        {
            return $"Withdrawn from the account:\n" +
                   $"Account ID: {AccountId}\n" +
                   $"Amount: {Amount}\n";
        }
    }
}