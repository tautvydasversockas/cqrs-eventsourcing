using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Commands
{
    public sealed class WithdrawFromAccount : Command
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public WithdrawFromAccount(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}