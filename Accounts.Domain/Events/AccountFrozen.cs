using System;
using Infrastructure.Domain;

namespace Accounts.Domain.Events
{
    public sealed class AccountFrozen : VersionedEvent<Guid>
    {
    }
}