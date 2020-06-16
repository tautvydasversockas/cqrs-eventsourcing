using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Events
{
    public sealed class AccountUnfrozen : Event
    {
        public Guid AccountId { get; }

        public AccountUnfrozen(Guid accountId)
        {
            AccountId = accountId;
        }

        public override string ToString()
        {
            return $"The account was unfrozen:\n" +
                   $"Account ID: {AccountId}\n";
        }
    }
}