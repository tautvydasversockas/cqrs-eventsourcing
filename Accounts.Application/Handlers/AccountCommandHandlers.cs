namespace Accounts.Application.Handlers;

public sealed class AccountCommandHandlers :
    IRequestHandler<OpenAccount, Guid>,
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

    public async Task<Guid> Handle(OpenAccount command, CancellationToken token = default)
    {
        var account = Account.Open(
            _repository.GetNextId(),
            new ClientId(command.ClientId),
            new InterestRate(command.InterestRate),
            command.Balance);

        await _repository.SaveAsync(account, token);

        return account.Id;
    }

    public async Task<Unit> Handle(DepositToAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            new AccountId(command.AccountId),
            account => account.Deposit(new Money(command.Amount)),
            token);
    }

    public async Task<Unit> Handle(WithdrawFromAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            new AccountId(command.AccountId),
            account => account.Withdraw(new Money(command.Amount)),
            token);
    }

    public async Task<Unit> Handle(AddInterestsToAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            new AccountId(command.AccountId),
            account => account.AddInterests(),
            token);
    }

    public async Task<Unit> Handle(FreezeAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            new AccountId(command.AccountId),
            account => account.Freeze(),
            token);
    }

    public async Task<Unit> Handle(UnfreezeAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            new AccountId(command.AccountId),
            account => account.Unfreeze(),
            token);
    }

    public async Task<Unit> Handle(CloseAccount command, CancellationToken token = default)
    {
        return await _repository.ExecuteAsync(
            new AccountId(command.AccountId),
            account => account.Close(),
            token);
    }
}
