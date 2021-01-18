using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Open_an_account
{
    public sealed class Opening_a_new_account_with_negative_balance : AccountSpecification<OpenAccount>
    {
        protected override OpenAccount When()
        {
            return new(AccountId, 0, -100);
        }

        protected override IEnumerable<IEvent> Then()
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}