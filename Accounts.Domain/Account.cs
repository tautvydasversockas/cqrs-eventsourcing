using System;
using System.Collections.Generic;
using Accounts.Domain.Events;
using Infrastructure.Domain;
using static Accounts.Domain.AccountStatus;

namespace Accounts.Domain
{
    public sealed class Account : EventSourcedAggregate<Guid>
    {
        private AccountStatus _status;
        private decimal _balance;
        private decimal _interestRate;

        private Account(Guid id)
            : base(id) { }

        public Account(Guid id, IEnumerable<IVersionedEvent<Guid>> events)
            : base(id, events) { }

        public static Account Open(
            Guid id,
            Guid clientId,
            decimal interestRate,
            decimal balance)
        {
            if (id == null)
                throw new InvalidOperationException("Account id must be provided");

            if (clientId == null)
                throw new InvalidOperationException("Client id must be provided");

            if (interestRate < 0 || interestRate > 1)
                throw new InvalidOperationException("Interest rate must be between 0 and 1");

            if (balance < 0)
                throw new InvalidOperationException("Balance cannot be negative");

            var account = new Account(id);
            account.Raise(new AccountOpened(clientId, interestRate, balance));
            return account;
        }

        public void Withdraw(decimal amount)
        {
            if (_status == Frozen)
                throw new InvalidOperationException("Cannot withdraw from frozen account");

            if (amount > _balance)
                throw new InvalidOperationException("Cannot withdraw more than existing balance");

            Raise(new WithdrawnFromAccount(amount));
        }

        public void Deposit(decimal amount)
        {
            if (_status == Frozen)
                throw new InvalidOperationException("Cannot deposit to frozen account");

            Raise(new DepositedToAccount(amount));
        }

        public void AddInterests()
        {
            if (_status == Frozen)
                throw new InvalidOperationException("Cannot add interests to frozen account");

            var interests = _balance * _interestRate;

            Raise(new AddedInterestsToAccount(interests));
        }

        public void Freeze()
        {
            if (_status == Frozen)
                throw new InvalidOperationException("Cannot freeze frozen account");

            Raise(new AccountFrozen());
        }

        public void Unfreeze()
        {
            if (_status != Frozen)
                throw new InvalidOperationException("Cannot unfreeze not frozen account");

            Raise(new AccountUnFrozen());
        }

        private void Apply(AccountOpened evt)
        {
            _status = Active;
            _balance = evt.Balance;
            _interestRate = evt.InterestRate;
        }

        private void Apply(WithdrawnFromAccount evt)
        {
            _balance -= evt.Amount;
        }

        private void Apply(DepositedToAccount evt)
        {
            _balance += evt.Amount;
        }

        private void Apply(AddedInterestsToAccount evt)
        {
            _balance += evt.Interests;
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