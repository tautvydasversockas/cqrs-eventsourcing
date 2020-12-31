using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Add_interests_to_an_account
{
    public sealed class Adding_interests_to_a_frozen_account : Specification<Account, Guid, AddInterestsToAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(_accountId);
        }

        protected override AddInterestsToAccount When()
        {
            return new(_accountId);
        }

        protected override bool Then_Fail() => true;
    }
}