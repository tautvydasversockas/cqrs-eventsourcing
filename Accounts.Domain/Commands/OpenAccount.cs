using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Commands
{
    public sealed class OpenAccount : Command
    {
        public Guid AccountId { get; }
        public Guid ClientId { get; }
        public decimal InterestRate { get; }
        public decimal Balance { get; }

        public OpenAccount(Guid accountId, Guid clientId, decimal interestRate, decimal balance)
        {
            AccountId = accountId;
            ClientId = clientId;
            InterestRate = interestRate;
            Balance = balance;
        }
    }
}