using System;

namespace Accounts.Domain
{
    public interface IEvent { }

    public sealed record AccountOpened(
        Guid AccountId,
        Guid ClientId,
        decimal InterestRate,
        decimal Balance) : IEvent;

    public sealed record AddedInterestsToAccount(
        Guid AccountId,
        decimal Interests) : IEvent;

    public sealed record DepositedToAccount(
        Guid AccountId,
        decimal Amount) : IEvent;

    public sealed record WithdrawnFromAccount(
        Guid AccountId,
        decimal Amount) : IEvent;

    public sealed record AccountFrozen(
        Guid AccountId) : IEvent;

    public sealed record AccountUnfrozen(
        Guid AccountId) : IEvent;
}