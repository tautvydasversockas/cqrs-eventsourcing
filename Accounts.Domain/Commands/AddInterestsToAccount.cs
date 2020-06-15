using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Commands
{
    public sealed class AddInterestsToAccount : Command
    {
        public Guid AccountId { get; }

        public AddInterestsToAccount(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}