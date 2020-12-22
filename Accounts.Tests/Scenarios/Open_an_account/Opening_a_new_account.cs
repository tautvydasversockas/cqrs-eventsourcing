using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Open_an_account
{
    public sealed class Opening_a_new_account : Specification<Account, Guid, OpenAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();
        private readonly Guid _clientId = Guid.NewGuid();

        protected override OpenAccount When()
        {
            return new(_accountId, _clientId, 0, 0);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new AccountOpened(_accountId, _clientId, 0, 0) { Version = 1 };
        }
    }
}