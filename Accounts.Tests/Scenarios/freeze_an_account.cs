using System;
using System.Collections.Generic;
using Accounts.Domain;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
using FluentAssertions;

namespace Accounts.Tests.Scenarios.freeze_an_account
{
    public sealed class freezing_an_active_account : Specification<Account, FreezeAccount>
    {
        private readonly Guid _id = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new AccountOpened(_id, Guid.NewGuid(), 0, 0);
        }

        protected override FreezeAccount When()
        {
            return new FreezeAccount(_id);
        }

        [Then]
        public void the_account_is_frozen()
        {
            AssertPublished<AccountFrozen>();
            var @event = GetFromPublished<AccountFrozen>();
            @event.AccountId.Should().Be(Command.AccountId);
        }
    }
}