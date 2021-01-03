using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Unfreeze_an_account
{
    public sealed class Unfreezing_a_frozen_account : AccountSpecification<UnfreezeAccount>
    {
        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(AccountId, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(AccountId);
        }

        protected override UnfreezeAccount When()
        {
            return new(AccountId);
        }

        protected override IEnumerable<IEvent> Then()
        {
            yield return new AccountUnfrozen(AccountId);
        }
    }
}