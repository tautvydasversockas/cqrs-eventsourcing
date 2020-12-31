using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_a_frozen_account : Specification<Account, Guid, WithdrawFromAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 200);
            yield return new AccountFrozen(_accountId);
        }

        protected override WithdrawFromAccount When()
        {
            return new(_accountId, 100);
        }

        protected override bool Then_Fail() => true;
    }
}