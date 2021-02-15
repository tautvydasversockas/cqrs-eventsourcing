using Accounts.Domain;
using Accounts.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Infrastructure
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly IEventSourcedRepository<Account, AccountId> _eventSourcedRepository;

        public AccountRepository(IEventSourcedRepository<Account, AccountId> eventSourcedRepository)
        {
            _eventSourcedRepository = eventSourcedRepository;
        }

        public AccountId GetNextIdentity()
        {
            return new(DeterministicGuid.Create(RequestContext.RequestIdNonNull));
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
}