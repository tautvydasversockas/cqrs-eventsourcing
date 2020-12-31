using System;

namespace Accounts.Domain
{
    public interface ICommand { };

    public sealed record OpenAccount(
        Guid AccountId,
        Guid ClientId,
        decimal InterestRate,
        decimal Balance) : ICommand;

    public sealed record AddInterestsToAccount(
        Guid AccountId) : ICommand;

    public sealed record DepositToAccount(
        Guid AccountId,
        decimal Amount) : ICommand;

    public sealed record WithdrawFromAccount(
        Guid AccountId,
        decimal Amount) : ICommand;

    public sealed record FreezeAccount(
        Guid AccountId) : ICommand;

    public sealed record UnfreezeAccount(
        Guid AccountId) : ICommand;
}