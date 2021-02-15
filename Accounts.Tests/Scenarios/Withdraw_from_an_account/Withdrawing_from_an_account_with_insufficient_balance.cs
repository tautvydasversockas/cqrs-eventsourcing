using Accounts.Domain;
using System;
using System.Collections.Generic;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_an_account_with_insufficient_balance : AccountSpecification<WithdrawFromAccount>
    {
        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(AccountId, Guid.NewGuid(), 0, 100);
        }

        protected override WithdrawFromAccount When()
        {
            return new(AccountId, 200);
        }

        protected override IEnumerable<IEvent> Then()
        {
            throw new InsufficientBalanceException();
        }
    }
}