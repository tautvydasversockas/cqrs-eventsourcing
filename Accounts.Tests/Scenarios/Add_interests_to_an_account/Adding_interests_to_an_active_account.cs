using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.Add_interests_to_an_account
{
    public sealed class Adding_interests_to_an_active_account : Specification<Account, AddInterestsToAccount>
    {
        private Guid _accountId;
        private decimal _interestRate;
        private decimal _balance;

        protected override void Before()
        {
            _accountId = Guid.NewGuid();
            _interestRate = 0.1m;
            _balance = 100;
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_accountId, Guid.NewGuid(), _interestRate, _balance);
        }

        protected override AddInterestsToAccount When()
        {
            return new AddInterestsToAccount(_accountId);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new AddedInterestsToAccount(_accountId, _interestRate * _balance);
        }
    }
}