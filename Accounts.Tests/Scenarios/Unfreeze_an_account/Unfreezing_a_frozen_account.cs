using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Unfreeze_an_account
{
    public sealed class Unfreezing_a_frozen_account : Specification<Account, UnfreezeAccount>
    {
        private Guid _accountId;

        protected override void Before()
        {
            _accountId = Guid.NewGuid();
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(_accountId);
        }

        protected override UnfreezeAccount When()
        {
            return new UnfreezeAccount(_accountId);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new AccountUnfrozen(_accountId);
        }
    }
}