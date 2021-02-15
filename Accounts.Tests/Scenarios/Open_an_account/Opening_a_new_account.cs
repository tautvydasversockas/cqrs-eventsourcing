using Accounts.Domain;
using System;
using System.Collections.Generic;

namespace Accounts.Tests.Scenarios.Open_an_account
{
    public sealed class Opening_a_new_account : AccountSpecification<OpenAccount>
    {
        private readonly Guid _clientId = Guid.NewGuid();

        protected override OpenAccount When()
        {
            return new(_clientId, 0, 0);
        }

        protected override IEnumerable<IEvent> Then()
        {
            yield return new AccountOpened(AccountId, _clientId, 0, 0);
        }
    }
}