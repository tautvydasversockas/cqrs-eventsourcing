namespace Accounts.Application.Commands;

public sealed record OpenAccount(
    ClientId ClientId,
    InterestRate InterestRate)
    : ICommand, IRequest<AccountId>;

public sealed record AddInterestsToAccount(
    AccountId AccountId)
    : ICommand, IRequest;

public sealed record DepositToAccount(
    AccountId AccountId,
    Money Money)
    : ICommand, IRequest;

public sealed record WithdrawFromAccount(
    AccountId AccountId,
    Money Money)
    : ICommand, IRequest;

public sealed record FreezeAccount(
    AccountId AccountId)
    : ICommand, IRequest;

public sealed record UnfreezeAccount(
    AccountId AccountId)
    : ICommand, IRequest;

public sealed record CloseAccount(
    AccountId AccountId)
    : ICommand, IRequest;
