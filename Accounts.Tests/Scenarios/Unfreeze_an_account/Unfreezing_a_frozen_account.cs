using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Unfreeze_an_account
{
    public sealed class Unfreezing_a_frozen_account : Specification<Account, UnfreezeAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0) { Version = 1 };
            yield return new AccountFrozen(_accountId) { Version = 2 };
        }

        protected override UnfreezeAccount When()
        {
            return new(_accountId);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new AccountUnfrozen(_accountId) { Version = 3 };
        }
    }
}