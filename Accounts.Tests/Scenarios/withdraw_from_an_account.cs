using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
using FluentAssertions;

namespace Accounts.Tests.Scenarios.withdraw_from_an_account
{
    public sealed class withdrawing_from_an_active_account : Specification<Account, WithdrawFromAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 200);
        }

        protected override WithdrawFromAccount When()
        {
            return new WithdrawFromAccount(_id, 100);
        }

        [Then]
        public void money_withdrawn_from_the_account()
        {
            AssertPublished<WithdrawnFromAccount>();
            var @event = GetFromPublished<WithdrawnFromAccount>();
            @event.AccountId.Should().Be(Command.AccountId);
            @event.Amount.Should().Be(Command.Amount);
        }
    }

    public sealed class withdrawing_from_an_account_with_not_sufficient_balance : Specification<Account, WithdrawFromAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 100);
        }

        protected override WithdrawFromAccount When()
        {
            return new WithdrawFromAccount(_id, 200);
        }

        [Then]
        public void failed_to_withdraw_from_the_account()
        {
            AssertFailed();
        }
    }

    public sealed class withdrawing_from_a_frozen_account : Specification<Account, WithdrawFromAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 200);
            yield return new AccountFrozen(_id);
        }

        protected override WithdrawFromAccount When()
        {
            return new WithdrawFromAccount(_id, 100);
        }

        [Then]
        public void failed_to_withdraw_from_the_account()
        {
            AssertFailed();
        }
    }
}