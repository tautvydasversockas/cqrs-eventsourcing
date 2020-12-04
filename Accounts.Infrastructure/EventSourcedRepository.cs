using System;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Domain.Common;

namespace Accounts.Infrastructure
{
    public sealed class EventSourcedRepository<TEventSourcedAggregate> : IEventSourcedRepository<TEventSourcedAggregate>
        where TEventSourcedAggregate : EventSourcedAggregate, new()
    {
        private readonly IEventStore _eventStore;

        public EventSourcedRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task SaveAsync(TEventSourcedAggregate aggregate)
        {
            if (!aggregate.UncommittedEvents.Any())
                return;

            await _eventStore.SaveAggregateEventsAsync<TEventSourcedAggregate>(aggregate.Id, aggregate.UncommittedEvents);

            aggregate.MarkEventsAsCommitted();
        }

        public async Task<TEventSourcedAggregate?> GetAsync(Guid id)
        {
            TEventSourcedAggregate? aggregate = null;

            await foreach (var @event in _eventStore.GetAggregateEventsAsync<TEventSourcedAggregate>(id))
                (aggregate ??= new TEventSourcedAggregate()).ApplyEvent(@event);

            return aggregate;
        }
    }
}