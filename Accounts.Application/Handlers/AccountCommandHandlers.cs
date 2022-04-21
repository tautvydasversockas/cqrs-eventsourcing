namespace Accounts.Application.Handlers;

public sealed class AccountCommandHandlers :
    IRequestHandler<OpenAccount, AccountId>,
    IRequestHandler<DepositToAccount>,
    IRequestHandler<WithdrawFromAccount>,
    IRequestHandler<AddInterestsToAccount>,
    IRequestHandler<FreezeAccount>,
    IRequestHandler<UnfreezeAccount>,
    IRequestHandler<CloseAccount>
{
    private readonly IAccountRepository _repository;

    public AccountCommandHandlers(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountId> Handle(OpenAccount command, CancellationToken token = default)
    {
        var account = Account.Open(
            _repository.GetNextId(),
            command.ClientId,
            command.InterestRate);

        await _repository.SaveAsync(account, token);

        return account.Id;
    }

    public async Task<Unit> Handle(DepositToAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            command.AccountId,
            account => account.Deposit(command.Money),
            token);
    }

    public async Task<Unit> Handle(WithdrawFromAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            command.AccountId,
            account => account.Withdraw(command.Money),
            token);
    }

    public async Task<Unit> Handle(AddInterestsToAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            command.AccountId,
            account => account.AddInterests(),
            token);
    }

    public async Task<Unit> Handle(FreezeAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            command.AccountId,
            account => account.Freeze(),
            token);
    }

    public async Task<Unit> Handle(UnfreezeAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            command.AccountId,
            account => account.Unfreeze(),
            token);
    }

    public async Task<Unit> Handle(CloseAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            command.AccountId,
            account => account.Close(),
            token);
    }
}
