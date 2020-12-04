using System;

namespace Accounts.ReadModel
{
    public sealed record AccountDto(
        Guid Id,
        int Version,
        Guid ClientId,
        decimal InterestRate,
        decimal Balance,
        bool IsFrozen);
}