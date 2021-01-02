using MediatR;
using System;

namespace Accounts.Domain
{
    public sealed record OpenAccount(
        Guid AccountId,
        Guid ClientId,
        decimal InterestRate,
        decimal Balance) : IRequest;

    public sealed record AddInterestsToAccount(
        Guid AccountId) : IRequest;

    public sealed record DepositToAccount(
        Guid AccountId,
        decimal Amount) : IRequest;

    public sealed record WithdrawFromAccount(
        Guid AccountId,
        decimal Amount) : IRequest;

    public sealed record FreezeAccount(
        Guid AccountId) : IRequest;

    public sealed record UnfreezeAccount(
        Guid AccountId) : IRequest;
}