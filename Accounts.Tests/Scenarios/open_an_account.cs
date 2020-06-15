using System;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Events;
using FluentAssertions;

namespace Accounts.Tests.Scenarios.open_an_account
{
    public sealed class opening_a_new_account : Specification<Account, OpenAccount>
    {
        protected override OpenAccount When()
        {
            return new OpenAccount(Guid.NewGuid(), Guid.NewGuid(), 0, 0);
        }

        [Then]
        public void the_account_is_opened()
        {
            AssertPublished<AccountOpened>();
            var @event = GetFromPublished<AccountOpened>();
            @event.AccountId.Should().Be(Command.AccountId);
            @event.ClientId.Should().Be(Command.ClientId);
            @event.InterestRate.Should().Be(Command.InterestRate);
            @event.Balance.Should().Be(Command.Balance);
        }
    }

    public sealed class opening_a_new_account_with_negative_balance : Specification<Account, OpenAccount>
    {
        protected override OpenAccount When()
        {
            return new OpenAccount(Guid.NewGuid(), Guid.NewGuid(), 0, -100);
        }

        [Then]
        public void failed_to_open_an_account()
        {
            AssertFailed();
        }
    }
}