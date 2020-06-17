using System;
using Accounts.Domain;
using Accounts.Domain.Commands;

namespace Accounts.Tests.Scenarios.Open_an_account
{
    public sealed class Opening_a_new_account_with_negative_balance : Specification<Account, OpenAccount>
    {
        protected override OpenAccount When()
        {
            return new OpenAccount(Guid.NewGuid(), Guid.NewGuid(), 0, -100);
        }

        protected override string Then_Fail()
        {
            return "Failed to open an account";
        }
    }
}