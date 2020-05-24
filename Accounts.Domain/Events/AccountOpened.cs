using System;
using Infrastructure.Domain;

namespace Accounts.Domain.Events
{
    public sealed class AccountOpened : VersionedEvent
    {
        public Guid ClientId { get; }
        public decimal InterestRate { get; }
        public decimal Balance { get; }

        public AccountOpened(Guid clientId, decimal interestRate, decimal balance)
        {
            ClientId = clientId;
            InterestRate = interestRate;
            Balance = balance;
        }
    }
}