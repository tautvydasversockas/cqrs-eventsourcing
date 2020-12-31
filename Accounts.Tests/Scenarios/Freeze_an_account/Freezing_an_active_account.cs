using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Freeze_an_account
{
    public sealed class Freezing_an_active_account : Specification<Account, Guid, FreezeAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
        }

        protected override FreezeAccount When()
        {
            return new(_accountId);
        }

        protected override IEnumerable<IEvent> Then()
        {
            yield return new AccountFrozen(_accountId);
        }
    }
}