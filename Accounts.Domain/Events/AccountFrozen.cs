using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Events
{
    public sealed class AccountFrozen : Event
    {
        public Guid AccountId { get; }

        public AccountFrozen(Guid accountId)
        {
            AccountId = accountId;
        }

        public override string ToString()
        {
            return $"The account was frozen:{Environment.NewLine}" +
                   $"Account ID: {AccountId}{Environment.NewLine}";
        }
    }
}