using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Domain.Common
{
    public interface IEventSourcedRepository<TEventSourcedAggregate, in TId>
        where TEventSourcedAggregate : EventSourcedAggregate<TId>, new()
        where TId : notnull
    {
        Task SaveAsync(TEventSourcedAggregate aggregate, CancellationToken token = default);
        Task<TEventSourcedAggregate?> GetAsync(TId id, CancellationToken token = default);
    }
}