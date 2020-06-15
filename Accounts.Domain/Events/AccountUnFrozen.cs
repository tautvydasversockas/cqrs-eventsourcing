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
    }
}