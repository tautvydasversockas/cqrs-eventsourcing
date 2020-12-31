using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_an_active_account : Specification<Account, Guid, WithdrawFromAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 200);
        }

        protected override WithdrawFromAccount When()
        {
            return new(_accountId, 100);
        }

        protected override IEnumerable<IEvent> Then()
        {
            yield return new WithdrawnFromAccount(_accountId, 100);
        }
    }
}