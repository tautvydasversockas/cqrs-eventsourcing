using System;
using System.Collections.Generic;
using Accounts.Domain.Events;
using Infrastructure.Domain;
using static Accounts.Domain.Account.Status;

namespace Accounts.Domain
{
    public sealed class Account : EventSourcedAggregate
    {
        public enum Status
        {
            Active,
            Frozen
        }

        private Status _status;
        private decimal _balance;
        private InterestRate _interestRate;

        private Account(Guid id)
            : base(id) { }

        public Account(Guid id, IEnumerable<IVersionedEvent> events)
            : base(id, events) { }

        public static Account Open(Guid id, Guid clientId, InterestRate interestRate, decimal balance)
        {
            if (id == default)
                throw new ArgumentException("Account id is required.");

            if (clientId == default)
                throw new ArgumentException("Client id is required.");

            if (balance < 0)
                throw new InvalidOperationException("Balance cannot be negative.");

            var account = new Account(id);
            account.Raise(new AccountOpened(clientId, interestRate, balance));
            return account;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be positive.");

            if (_status == Frozen)
                throw new InvalidOperationException("Cannot withdraw from frozen account.");

            if (amount > _balance)
                throw new InvalidOperationException("Cannot withdraw more than balance.");

            Raise(new WithdrawnFromAccount(amount));
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be positive.");

            if (_status == Frozen)
                throw new InvalidOperationException("Cannot deposit to frozen account.");

            Raise(new DepositedToAccount(amount));
        }

        public void AddInterests()
        {
            if (_status == Frozen)
                throw new InvalidOperationException("Cannot add interests to frozen account.");

            var interests = _balance * _interestRate;

            Raise(new AddedInterestsToAccount(interests));
        }

        public void Freeze()
        {
            if (_status == Frozen)
                return;

            Raise(new AccountFrozen());
        }

        public void Unfreeze()
        {
            if (_status != Frozen)
                return;

            Raise(new AccountUnfrozen());
        }

        private void Apply(AccountOpened @event)
        {
            _status = Active;
            _balance = @event.Balance;
            _interestRate = (InterestRate)@event.InterestRate;
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
    }
}