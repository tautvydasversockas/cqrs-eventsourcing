using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Deposit_to_an_account
{
    public sealed class Depositing_to_a_frozen_account : Specification<Account, Guid, DepositToAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(_accountId);
        }

        protected override DepositToAccount When()
        {
            return new(_accountId, 100);
        }

        protected override bool Then_Fail() => true;
    }
}