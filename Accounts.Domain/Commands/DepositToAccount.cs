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

        public override string ToString()
        {
            return $"Depositing to the account:\n" +
                   $"Account ID: {AccountId}\n" +
                   $"Amount: {Amount}\n";
        }
    }
}