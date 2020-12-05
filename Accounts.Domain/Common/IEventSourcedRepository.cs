using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Domain.Common
{
    public interface IEventSourcedRepository<TEventSourcedAggregate> 
        where TEventSourcedAggregate : EventSourcedAggregate, new()
    {
        Task SaveAsync(TEventSourcedAggregate aggregate, CancellationToken token = default);
        Task<TEventSourcedAggregate?> GetAsync(Guid id, CancellationToken token = default);
    }
}