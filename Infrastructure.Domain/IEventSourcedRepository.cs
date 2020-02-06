using System.Threading.Tasks;

namespace Infrastructure.Domain
{
    public interface IEventSourcedRepository<TEventSourcedAggregate, in TId> where TEventSourcedAggregate : EventSourcedAggregate<TId>
    {
        Task SaveAsync(TEventSourcedAggregate aggregate, string correlationId);
        Task<TEventSourcedAggregate> GetAsync(TId id);
    }
}