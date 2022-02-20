namespace Accounts.Infrastructure;

public sealed class AccountRepository : IAccountRepository
{
    private readonly IEventSourcedRepository<Account, AccountId> _eventSourcedRepository;

    public AccountRepository(IEventSourcedRepository<Account, AccountId> eventSourcedRepository)
    {
        _eventSourcedRepository = eventSourcedRepository;
    }

    public AccountId GetNextId()
    {
        return new(DeterministicGuid.Create(RequestContext.RequestId ?? Guid.NewGuid()));
    }

    public Task<Account?> GetAsync(AccountId id, CancellationToken token = default)
    {
        return _eventSourcedRepository.GetAsync(id, token);
    }

    public Task SaveAsync(Account aggregate, CancellationToken token = default)
    {
        return _eventSourcedRepository.SaveAsync(aggregate, token);
    }
}
