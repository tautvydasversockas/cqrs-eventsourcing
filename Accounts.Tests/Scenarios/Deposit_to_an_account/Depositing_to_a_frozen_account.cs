using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Deposit_to_an_account
{
    public sealed class Depositing_to_a_frozen_account : Specification<Account, DepositToAccount>
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

        protected override DepositToAccount When()
        {
            return new DepositToAccount(_accountId, 100);
        }

        protected override string Then_Fail()
        {
            return "Failed to deposit to the account";
        }
    }
}