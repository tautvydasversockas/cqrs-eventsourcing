using System;
using Infrastructure.Domain;

namespace Accounts.Domain.Events
{
    public sealed class AddedInterestsToAccount : VersionedEvent<Guid>
    {
        public decimal Interests { get; }

        public AddedInterestsToAccount(decimal interests)
        {
            Interests = interests;
        }
    }
}