using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Domain.Common;
using Accounts.Infrastructure.Exceptions;
using EventStore.ClientAPI;

namespace Accounts.Infrastructure
{
    public sealed class EventStore : IEventStore
    {
        private readonly IEventStoreConnection _connection;
        private readonly MessageContext _context;

        public EventStore(IEventStoreConnection connection, MessageContext context)
        {
            _connection = connection;
            _context = context;
        }

        public async IAsyncEnumerable<Event> GetAggregateEventsAsync<TEventSourcedAggregate>(Guid aggregateId)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            var streamName = GetAggregateStreamName<TEventSourcedAggregate>(aggregateId);
            StreamEventsSlice currentSlice;
            var nextSliceStart = (long)StreamPosition.Start;

            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 10, false);
                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var resolvedEvent in currentSlice.Events)
                {
                    var (@event, metadata) = EventStoreSerializer.Deserialize(resolvedEvent);

                    if (_context.MessageId == metadata.CausationId)
                        throw new DuplicateOperationException(_context.CausationId);

                    yield return @event;
                }
            } while (!currentSlice.IsEndOfStream);
        }

        public async Task SaveAggregateEventsAsync<TEventSourcedAggregate>(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            var streamName = GetAggregateStreamName<TEventSourcedAggregate>(aggregateId);
            var metadata = new Metadata(_context.CausationId, _context.CorrelationId);
            var eventsToSave = events.Select(@event => EventStoreSerializer.Serialize(@event, metadata));
            await _connection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
        }

        private static string GetAggregateStreamName<TEventSourcedAggregate>(Guid aggregateId)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            return $"{typeof(TEventSourcedAggregate).Name}-{aggregateId}";
        }
    }
}