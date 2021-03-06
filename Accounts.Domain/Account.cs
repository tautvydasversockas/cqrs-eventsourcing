﻿using Accounts.Domain.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using static Accounts.Domain.Account.Status;

namespace Accounts.Domain
{
    public sealed class Account : EventSourcedAggregate<AccountId>
    {
        public enum Status
        {
            Active,
            Frozen,
            Closed
        }

        private Status _status;
        private decimal _balance;
        [AllowNull] private InterestRate _interestRate;

        private Account() { }

        public static Account Open(
            AccountId id,
            ClientId clientId,
            InterestRate interestRate,
            decimal balance)
        {
            if (balance < 0)
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance can't be negative.");

            var account = new Account();
            account.Raise(new AccountOpened(id, clientId, interestRate, balance));
            return account;
        }

        public void Withdraw(Money money)
        {
            if (_status is Closed)
                throw new ClosedAccountException();

            if (_status is Frozen)
                throw new FrozenAccountException();

            if (money > _balance)
                throw new InsufficientBalanceException();

            Raise(new WithdrawnFromAccount(Id, money));
        }

        public void Deposit(Money money)
        {
            if (_status is Closed)
                throw new ClosedAccountException();

            if (_status is Frozen)
                throw new FrozenAccountException();

            Raise(new DepositedToAccount(Id, money));
        }

        public void AddInterests()
        {
            if (_status is Closed)
                throw new ClosedAccountException();

            if (_status is Frozen)
                throw new FrozenAccountException();

            var interests = _balance * _interestRate;

            Raise(new AddedInterestsToAccount(Id, interests));
        }

        public void Freeze()
        {
            if (_status is Closed)
                throw new ClosedAccountException();

            if (_status is Frozen)
                return;

            Raise(new AccountFrozen(Id));
        }

        public void Unfreeze()
        {
            if (_status is Closed)
                throw new ClosedAccountException();

            if (_status is Active)
                return;

            Raise(new AccountUnfrozen(Id));
        }

        public void Close()
        {
            if (_status is Closed)
                return;

            Raise(new AccountClosed(Id));
        }

        private void Apply(AccountOpened @event)
        {
            Id = new AccountId(@event.AccountId);
            _status = Active;
            _balance = @event.Balance;
            _interestRate = new InterestRate(@event.InterestRate);
        }

        private void Apply(WithdrawnFromAccount @event)
        {
            _balance -= @event.Amount;
        }

        private void Apply(DepositedToAccount @event)
        {
            _balance += @event.Amount;
        }

        private void Apply(AddedInterestsToAccount @event)
        {
            _balance += @event.Interests;
        }

        private void Apply(AccountFrozen @event)
        {
            _status = Frozen;
        }

        private void Apply(AccountUnfrozen @event)
        {
            _status = Active;
        }

        private void Apply(AccountClosed @event)
        {
            _status = Closed;
        }
    }
}