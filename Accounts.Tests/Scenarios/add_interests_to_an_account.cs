using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
using FluentAssertions;

namespace Accounts.Tests.Scenarios.add_interests_to_an_account
{
    public sealed class adding_interests_to_an_active_account : Specification<Account, AddInterestsToAccount>
    {
        private readonly Guid _id = Guid.NewGuid();
        private const decimal _interestRate = 0.1m;
        private const decimal _balance = 100;

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), _interestRate, _balance);
        }

        protected override AddInterestsToAccount When()
        {
            return new AddInterestsToAccount(_id);
        }

        [Then]
        public void interests_added_to_the_account()
        {
            AssertPublished<AddedInterestsToAccount>();
            var @event = GetFromPublished<AddedInterestsToAccount>();
            @event.AccountId.Should().Be(Command.AccountId);
            @event.Interests.Should().Be(_interestRate * _balance);
        }
    }

    public sealed class adding_interests_to_a_frozen_account : Specification<Account, AddInterestsToAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(_id);
        }

        protected override AddInterestsToAccount When()
        {
            return new AddInterestsToAccount(_id);
        }

        [Then]
        public void failed_to_add_interests_to_the_account()
        {
            AssertFailed();
        }
    }
}