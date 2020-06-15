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
    }
}