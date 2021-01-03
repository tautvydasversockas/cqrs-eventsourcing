using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_a_frozen_account : AccountSpecification<WithdrawFromAccount>
    {
        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(AccountId, Guid.NewGuid(), 0, 200);
            yield return new AccountFrozen(AccountId);
        }

        protected override WithdrawFromAccount When()
        {
            return new(AccountId, 100);
        }

        protected override IEnumerable<IEvent> Then()
        {
            throw new InvalidOperationException();
        }
    }
}