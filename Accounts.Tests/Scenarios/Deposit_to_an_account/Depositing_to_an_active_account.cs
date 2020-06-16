using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Deposit_to_an_account
{
    public sealed class Depositing_to_an_active_account : Specification<Account, DepositToAccount>
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
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
        }

        protected override DepositToAccount When()
        {
            return new DepositToAccount(_accountId, _amount);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new DepositedToAccount(_accountId, _amount);
        }
    }
}