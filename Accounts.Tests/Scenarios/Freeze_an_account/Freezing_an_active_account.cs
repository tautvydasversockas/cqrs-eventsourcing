using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Freeze_an_account
{
    public sealed class Freezing_an_active_account : AccountSpecification<FreezeAccount>
    {
        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(AccountId, Guid.NewGuid(), 0, 0);
        }

        protected override FreezeAccount When()
        {
            return new(AccountId);
        }

        protected override IEnumerable<IEvent> Then()
        {
            yield return new AccountFrozen(AccountId);
        }
    }
}