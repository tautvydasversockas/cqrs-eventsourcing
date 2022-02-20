namespace Accounts.Domain;

public interface IAccountRepository : IEventSourcedRepository<Account, AccountId>
{
    AccountId GetNextId();
}
