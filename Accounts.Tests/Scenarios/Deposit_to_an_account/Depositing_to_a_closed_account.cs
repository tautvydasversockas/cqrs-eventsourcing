using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Deposit_to_an_account
{
    public sealed class Depositing_to_a_closed_account : AccountSpecification<DepositToAccount>
    {
        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(AccountId, Guid.NewGuid(), 0, 0);
            yield return new AccountClosed(AccountId);
        }

        protected override DepositToAccount When()
        {
            return new(AccountId, 100);
        }

        protected override IEnumerable<IEvent> Then()
        {
            throw new ClosedAccountException();
        }
    }
}