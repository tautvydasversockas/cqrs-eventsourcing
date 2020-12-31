using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Unfreeze_an_account
{
    public sealed class Unfreezing_a_frozen_account : Specification<Account, Guid, UnfreezeAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(_accountId);
        }

        protected override UnfreezeAccount When()
        {
            return new(_accountId);
        }

        protected override IEnumerable<IEvent> Then()
        {
            yield return new AccountUnfrozen(_accountId);
        }
    }
}