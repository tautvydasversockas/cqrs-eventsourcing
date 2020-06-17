using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_an_account_with_not_sufficient_balance : Specification<Account, WithdrawFromAccount>
    {
        private Guid _accountId;

        protected override void Before()
        {
            _accountId = Guid.NewGuid();
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 100);
        }

        protected override WithdrawFromAccount When()
        {
            return new WithdrawFromAccount(_accountId, 200);
        }

        protected override string Then_Fail()
        {
            return "Failed to withdraw from the account";
        }
    }
}