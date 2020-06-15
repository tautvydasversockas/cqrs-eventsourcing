using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
using FluentAssertions;

namespace Accounts.Tests.Scenarios.unfreeze_an_account
{
    public sealed class unfreezing_a_frozen_account : Specification<Account, UnfreezeAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 0);
            yield return new AccountFrozen(_id);
        }

        protected override UnfreezeAccount When()
        {
            return new UnfreezeAccount(_id);
        }

        [Then]
        public void the_account_is_unfrozen()
        {
            AssertPublished<AccountUnfrozen>();
            var @event = GetFromPublished<AccountUnfrozen>();
            @event.AccountId.Should().Be(Command.AccountId);
        }
    }
}