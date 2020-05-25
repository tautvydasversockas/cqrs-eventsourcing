using System;
using System.Linq;
using Accounts.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Accounts.Domain.Tests
{
    public sealed class AccountTests
    {
        [Fact]
        public void Open_OpensAccount()
        {
            var clientId = Guid.NewGuid();
            var interestRate = (InterestRate)0.1m;
            var balance = 0;

            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: clientId,
                interestRate: interestRate,
                balance: balance);

            account.GetUncommittedEvents().SingleOrDefault().Should().Match<AccountOpened>(e =>
                e.SourceId == account.Id &&
                e.ClientId == clientId &&
                e.InterestRate == interestRate &&
                e.Balance == balance);
        }

        [Fact]
        public void Open_NegativeBalance_Throws()
        {
            Func<Account> openAccount = () => Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: -1);

            openAccount.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Withdraw_ActiveAccountAndSufficientBalance_WithdrawsMoneyFromAccount()
        {
            var amount = 1;

            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: amount + 1);

            account.MarkEventsAsCommitted();

            account.Withdraw(amount);

            account.GetUncommittedEvents().SingleOrDefault().Should().Match<WithdrawnFromAccount>(e =>
                e.SourceId == account.Id &&
                e.Amount == amount);
        }

        [Fact]
        public void Withdraw_NotSufficientBalance_Throws()
        {
            var amount = 1;

            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: amount - 1);

            account.MarkEventsAsCommitted();

            Action withdraw = () => account.Withdraw(amount);

            withdraw.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Withdraw_FrozenAccount_Throws()
        {
            var amount = 1;

            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: amount + 1);

            account.Freeze();
            account.MarkEventsAsCommitted();

            Action withdraw = () => account.Withdraw(amount);

            withdraw.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Deposit_ActiveAccount_DepositsMoneyToAccount()
        {
            var amount = 1;

            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: 0);

            account.MarkEventsAsCommitted();

            account.Deposit(amount);

            account.GetUncommittedEvents().SingleOrDefault().Should().Match<DepositedToAccount>(e =>
                e.SourceId == account.Id &&
                e.Amount == amount);
        }

        [Fact]
        public void Deposit_FrozenAccount_Throws()
        {
            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: 0);

            account.Freeze();
            account.MarkEventsAsCommitted();

            Action deposit = () => account.Deposit(1);

            deposit.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AddInterests_ActiveAccountAndPositiveBalance_AddsInterests()
        {
            var balance = 100;
            var interestRate = (InterestRate)0.1m;
            var interests = interestRate * balance;

            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: interestRate,
                balance: balance);

            account.MarkEventsAsCommitted();

            account.AddInterests();

            account.GetUncommittedEvents().SingleOrDefault().Should().Match<AddedInterestsToAccount>(e =>
                e.SourceId == account.Id &&
                e.Interests == interests);
        }

        [Fact]
        public void AddInterests_FrozenAccount_Throws()
        {
            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: 0);

            account.Freeze();
            account.MarkEventsAsCommitted();

            Action addInterests = () => account.AddInterests();

            addInterests.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Freeze_ActiveAccount_FreezesAccount()
        {
            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: 0);

            account.MarkEventsAsCommitted();

            account.Freeze();

            account.GetUncommittedEvents().SingleOrDefault().Should().Match<AccountFrozen>(e =>
                e.SourceId == account.Id);
        }

        [Fact]
        public void Unfreeze_FrozenAccount_UnfreezesAccount()
        {
            var account = Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: 0);

            account.Freeze();
            account.MarkEventsAsCommitted();

            account.Unfreeze();

            account.GetUncommittedEvents().SingleOrDefault().Should().Match<AccountUnfrozen>(e =>
                e.SourceId == account.Id);
        }
    }
}