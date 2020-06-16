using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Events
{
    public sealed class AccountOpened : Event
    {
        public Guid AccountId { get; }
        public Guid ClientId { get; }
        public decimal InterestRate { get; }
        public decimal Balance { get; }

        public AccountOpened(Guid accountId, Guid clientId, decimal interestRate, decimal balance)
        {
            AccountId = accountId;
            ClientId = clientId;
            InterestRate = interestRate;
            Balance = balance;
        }

        public override string ToString()
        {
            return $"The account was opened:\n" +
                   $"Account ID: {AccountId}\n" +
                   $"Client ID: {ClientId}\n" +
                   $"Interest rate: {InterestRate}\n" +
                   $"Balance: {Balance}\n";
        }
    }
}