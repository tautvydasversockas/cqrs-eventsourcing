using System;

namespace Accounts.Domain
{
    public abstract record Command;

    public sealed record OpenAccount(
        Guid AccountId,
        Guid ClientId,
        decimal InterestRate,
        decimal Balance) : Command;

    public sealed record AddInterestsToAccount(
        Guid AccountId) : Command;

    public sealed record DepositToAccount(
        Guid AccountId,
        decimal Amount) : Command;

    public sealed record WithdrawFromAccount(
        Guid AccountId,
        decimal Amount) : Command;

    public sealed record FreezeAccount(
        Guid AccountId) : Command;

    public sealed record UnfreezeAccount(
        Guid AccountId) : Command;
}