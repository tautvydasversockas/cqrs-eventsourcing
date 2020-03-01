using System;
using System.Threading.Tasks;

namespace Infrastructure.Domain
{
    public interface IEventSourcedRepository<TEventSourcedAggregate> where TEventSourcedAggregate : EventSourcedAggregate
    {
        Task SaveAsync(TEventSourcedAggregate aggregate, string correlationId);
        Task<TEventSourcedAggregate> GetAsync(Guid id);
    }
}