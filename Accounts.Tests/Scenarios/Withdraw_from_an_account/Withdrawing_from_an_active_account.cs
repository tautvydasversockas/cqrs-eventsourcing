using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_an_active_account : Specification<Account, WithdrawFromAccount>
    {
        private Guid _accountId;
        private decimal _amount;

        protected override void Before()
        {
            _accountId = Guid.NewGuid();
            _amount = 100;
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 200);
        }

        protected override WithdrawFromAccount When()
        {
            return new WithdrawFromAccount(_accountId, _amount);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new WithdrawnFromAccount(_accountId, _amount);
        }
    }
}