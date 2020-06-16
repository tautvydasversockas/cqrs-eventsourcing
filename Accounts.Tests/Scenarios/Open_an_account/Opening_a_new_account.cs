using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;

namespace Accounts.Tests.Scenarios.open_an_account
{
    public sealed class Opening_a_new_account : Specification<Account, OpenAccount>
    {
        private Guid _accountId;
        private Guid _clientId;
        private decimal _interestRate;
        private decimal _balance;

        protected override void Before()
        {
            _accountId = Guid.NewGuid();
            _clientId = Guid.NewGuid();
            _interestRate = 0;
            _balance = 0;
        }

        protected override OpenAccount When()
        {
            return new OpenAccount(_accountId, _clientId, _interestRate, _balance);
        }

        protected override IEnumerable<Event> Then()
        {
            yield return new AccountOpened(_accountId, _clientId, _interestRate, _balance);
        }
    }
}