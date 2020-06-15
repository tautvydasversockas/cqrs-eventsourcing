using System;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
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

        public static Account Open(Guid id, Guid clientId, InterestRate interestRate, decimal balance)
        {
            if (balance < 0)
                throw new InvalidOperationException("Balance cannot be negative.");

            var account = new Account();
            account.Raise(new AccountOpened(id, clientId, interestRate, balance));
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

            Raise(new WithdrawnFromAccount(Id, amount));
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be positive.");

            if (_status == Frozen)
                throw new InvalidOperationException("Cannot deposit to frozen account.");

            Raise(new DepositedToAccount(Id, amount));
        }

        public void AddInterests()
        {
            if (_status == Frozen)
                throw new InvalidOperationException("Cannot add interests to frozen account.");

            var interests = _balance * _interestRate;

            Raise(new AddedInterestsToAccount(Id, interests));
        }

        public void Freeze()
        {
            if (_status == Frozen)
                return;

            Raise(new AccountFrozen(Id));
        }

        public void Unfreeze()
        {
            if (_status != Frozen)
                return;

            Raise(new AccountUnfrozen(Id));
        }

        private void Apply(AccountOpened @event)
        {
            Id = @event.AccountId;
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