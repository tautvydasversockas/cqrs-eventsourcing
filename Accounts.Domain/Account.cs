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
        private Money _balance;
        private InterestRate _interestRate;

        private Account(Guid id)
            : base(id) { }

        public Account(Guid id, IEnumerable<IVersionedEvent> events)
            : base(id, events) { }

        public static Account Open(
            Guid id,
            Guid clientId,
            InterestRate interestRate,
            Money balance)
        {
            if (id == default)
                throw new ArgumentException("Account id is required.");

            if (clientId == default)
                throw new ArgumentException("Client id is required.");

            var account = new Account(id);
            account.Raise(new AccountOpened(clientId, interestRate, balance));
            return account;
        }

        public void Withdraw(Money amount)
        {
            if (_status == Frozen)
                throw new InvalidOperationException("Cannot withdraw from frozen account.");

            if (amount > _balance)
                throw new InvalidOperationException("Cannot withdraw more than balance.");

            Raise(new WithdrawnFromAccount(amount));
        }

        public void Deposit(Money amount)
        {
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

            Raise(new AccountUnFrozen());
        }

        private void Apply(AccountOpened evt)
        {
            _status = Active;
            _balance = (Money)evt.Balance;
            _interestRate = (InterestRate)evt.InterestRate;
        }

        private void Apply(WithdrawnFromAccount evt)
        {
            _balance -= (Money)evt.Amount;
        }

        private void Apply(DepositedToAccount evt)
        {
            _balance += (Money)evt.Amount;
        }

        private void Apply(AddedInterestsToAccount evt)
        {
            _balance += (Money)evt.Interests;
        }

        private void Apply(AccountFrozen evt)
        {
            _status = Frozen;
        }

        private void Apply(AccountUnFrozen evt)
        {
            _status = Active;
        }
    }
}