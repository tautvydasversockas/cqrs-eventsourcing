using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Commands
{
    public sealed class DepositToAccount : Command
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public DepositToAccount(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}