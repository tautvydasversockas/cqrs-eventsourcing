using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Commands
{
    public sealed class UnfreezeAccount : Command
    {
        public Guid AccountId { get; }

        public UnfreezeAccount(Guid accountId)
        {
            AccountId = accountId;
        }

        public override string ToString()
        {
            return $"Unfreezing the account:\n" +
                   $"Account ID: {AccountId}\n";
        }
    }
}