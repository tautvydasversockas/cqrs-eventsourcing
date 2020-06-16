using System;
using Accounts.Domain.Common;

namespace Accounts.Domain.Events
{
    public sealed class AddedInterestsToAccount : Event
    {
        public Guid AccountId { get; }
        public decimal Interests { get; }

        public AddedInterestsToAccount(Guid accountId, decimal interests)
        {
            AccountId = accountId;
            Interests = interests;
        }

        public override string ToString()
        {
            return $"Added interests to the account:\n" +
                   $"Account ID: {AccountId}\n" +
                   $"Interests: {Interests}\n";
        }
    }
}