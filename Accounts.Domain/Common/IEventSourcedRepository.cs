namespace Accounts.Domain.Common;

public interface IEventSourcedRepository<TEventSourcedAggregate, TId>
    where TEventSourcedAggregate : EventSourcedAggregate<TId>
{
    Task SaveAsync(TEventSourcedAggregate aggregate, CancellationToken token = default);
    Task<TEventSourcedAggregate?> GetAsync(TId id, CancellationToken token = default);
}
