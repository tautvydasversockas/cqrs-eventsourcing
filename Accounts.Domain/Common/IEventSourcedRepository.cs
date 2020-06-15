using System;
using System.Threading.Tasks;

namespace Accounts.Domain.Common
{
    public interface IEventSourcedRepository<TEventSourcedAggregate> where TEventSourcedAggregate : EventSourcedAggregate, new()
    {
        Task SaveAsync(TEventSourcedAggregate aggregate);
        Task<TEventSourcedAggregate?> GetAsync(Guid id);
    }
}