using System;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Open_an_account
{
    public sealed class Opening_a_new_account_with_negative_balance : Specification<Account, Guid, OpenAccount>
    {
        protected override OpenAccount When()
        {
            return new(Guid.NewGuid(), Guid.NewGuid(), 0, -100);
        }

        protected override bool Then_Fail() => true;
    }
}