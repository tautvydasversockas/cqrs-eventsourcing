using System;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Domain.Common;
using Accounts.Infrastructure.Exceptions;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;

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
            var uncommittedEvents = aggregate.GetUncommittedEvents();
            if (!uncommittedEvents.Any())
                return;

            var originalVersion = aggregate.Version - uncommittedEvents.Count;
            var expectedVersion = originalVersion == 0
                ? ExpectedVersion.NoStream
                : originalVersion - 1;

            try
            {
                await _eventStore.SaveAggregateEventsAsync<TEventSourcedAggregate>(aggregate.Id, uncommittedEvents, expectedVersion);
            }
            catch (WrongExpectedVersionException e)
                when (e.ExpectedVersion == ExpectedVersion.NoStream)
            {
                throw new DuplicateKeyException(aggregate.Id);
            }

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