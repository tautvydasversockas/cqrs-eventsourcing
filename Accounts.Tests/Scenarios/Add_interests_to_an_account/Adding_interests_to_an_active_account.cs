using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Add_interests_to_an_account
{
    public sealed class Adding_interests_to_an_active_account : Specification<Account, Guid, AddInterestsToAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0.1m, 100) { Version = 1 };
        }

        protected override AddInterestsToAccount When()
        {
            return new(_accountId);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new AddedInterestsToAccount(_accountId, 10) { Version = 2 };
        }
    }
}