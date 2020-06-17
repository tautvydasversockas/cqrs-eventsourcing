using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_a_frozen_account : Specification<Account, WithdrawFromAccount>
    {
        private Guid _accountId;

        protected override void Before()
        {
            _accountId = Guid.NewGuid();
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 200);
            yield return new AccountFrozen(_accountId);
        }

        protected override WithdrawFromAccount When()
        {
            return new WithdrawFromAccount(_accountId, 100);
        }

        protected override string Then_Fail()
        {
            return "Failed to withdraw from the account";
        }
    }
}