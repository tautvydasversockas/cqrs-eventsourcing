using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Freeze_an_account
{
    public sealed class Freezing_an_active_account : Specification<Account, FreezeAccount>
    {
        private Guid _accountId;

        protected override void Before()
        {
            _accountId = Guid.NewGuid();
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
        }

        protected override FreezeAccount When()
        {
            return new FreezeAccount(_accountId);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new AccountFrozen(_accountId);
        }
    }
}