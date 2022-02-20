namespace Accounts.Domain;

public interface ICommand { }

public sealed record OpenAccount(
    Guid ClientId,
    decimal InterestRate,
    decimal Balance) 
    : ICommand, IRequest<Guid>;

public sealed record AddInterestsToAccount(
    Guid AccountId) 
    : ICommand, IRequest;

public sealed record DepositToAccount(
    Guid AccountId,
    decimal Amount) 
    : ICommand, IRequest;

public sealed record WithdrawFromAccount(
    Guid AccountId,
    decimal Amount) 
    : ICommand, IRequest;

public sealed record FreezeAccount(
    Guid AccountId) 
    : ICommand, IRequest;

public sealed record UnfreezeAccount(
    Guid AccountId) 
    : ICommand, IRequest;

public sealed record CloseAccount(
    Guid AccountId) 
    : ICommand, IRequest;
