using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Deposit_to_an_account
{
    public sealed class Depositing_to_an_active_account : Specification<Account, Guid, DepositToAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
        }

        protected override DepositToAccount When()
        {
            return new(_accountId, 100);
        }

        protected override IEnumerable<IEvent> Then()
        {
            yield return new DepositedToAccount(_accountId, 100);
        }
    }
}