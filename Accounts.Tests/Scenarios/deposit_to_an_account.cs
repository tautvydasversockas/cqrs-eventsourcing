using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
using FluentAssertions;

namespace Accounts.Tests.Scenarios.deposit_to_an_account
{
    public sealed class depositing_to_an_active_account : Specification<Account, DepositToAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 0);
        }

        protected override DepositToAccount When()
        {
            return new DepositToAccount(_id, 100);
        }

        [Then]
        public void money_deposited_to_the_account()
        {
            AssertPublished<DepositedToAccount>();
            var @event = GetFromPublished<DepositedToAccount>();
            @event.AccountId.Should().Be(Command.AccountId);
            @event.Amount.Should().Be(Command.Amount);
        }
    }

    public sealed class depositing_to_a_frozen_account : Specification<Account, DepositToAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(_id);
        }

        protected override DepositToAccount When()
        {
            return new DepositToAccount(_id, 100);
        }

        [Then]
        public void failed_to_deposit_to_the_account()
        {
            AssertFailed();
        }
    }
}