using System;

namespace Accounts.Domain
{
    public abstract record Event
    {
        public int Version { get; set; }
    }

    public sealed record AccountOpened(
        Guid AccountId,
        Guid ClientId,
        decimal InterestRate,
        decimal Balance) : Event;

    public sealed record AddedInterestsToAccount(
        Guid AccountId,
        decimal Interests) : Event;

    public sealed record DepositedToAccount(
        Guid AccountId,
        decimal Amount) : Event;

    public sealed record WithdrawnFromAccount(
        Guid AccountId,
        decimal Amount) : Event;

    public sealed record AccountFrozen(
        Guid AccountId) : Event;

    public sealed record AccountUnfrozen(
        Guid AccountId) : Event;
}