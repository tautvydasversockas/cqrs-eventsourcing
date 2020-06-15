using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Commands
{
    public sealed class FreezeAccount : Command
    {
        public Guid AccountId { get; }

        public FreezeAccount(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}