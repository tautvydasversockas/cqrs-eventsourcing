using Accounts.Domain.Common;

namespace Accounts.Domain
{
    public interface IAccountRepository : IEventSourcedRepository<Account, AccountId>
    {
        AccountId GetNextIdentity();
    }
}