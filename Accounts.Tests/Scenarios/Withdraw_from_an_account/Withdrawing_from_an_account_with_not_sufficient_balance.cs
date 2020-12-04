﻿using System;
using System.Collections.Generic;
using Accounts.Domain;

namespace Accounts.Tests.Scenarios.Withdraw_from_an_account
{
    public sealed class Withdrawing_from_an_account_with_not_sufficient_balance : Specification<Account, WithdrawFromAccount>
    {
        private readonly Guid _accountId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), 0, 100) { Version = 1 };
        }

        protected override WithdrawFromAccount When()
        {
            return new(_accountId, 200);
        }

        protected override bool Then_Fail() => true;
    }
}