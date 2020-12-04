using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Domain;
using Accounts.Domain.Common;
using Accounts.Infrastructure.Exceptions;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;

namespace Accounts.Infrastructure
{
    public sealed class EventStore : IEventStore
    {
        private readonly IEventStoreConnection _connection;

        public EventStore(IEventStoreConnection connection)
        {
            _connection = connection;
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

                    if (MessageContext.MessageId is not null && MessageContext.MessageId == metadata.CausationId)
                        throw new DuplicateOperationException(MessageContext.MessageId);

                    yield return @event;
                }
            } while (!currentSlice.IsEndOfStream);
        }

        public async Task SaveAggregateEventsAsync<TEventSourcedAggregate>(Guid aggregateId, IReadOnlyCollection<Event> events)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            var streamName = GetAggregateStreamName<TEventSourcedAggregate>(aggregateId);
            var originalVersion = events.Min(@event => @event.Version) - 1;
            var expectedVersion = originalVersion - 1;
            var metadata = new Metadata(MessageContext.CausationId, MessageContext.CorrelationId);
            var eventsToSave = events.Select(@event => EventStoreSerializer.Serialize(@event, metadata));

            try
            {
                await _connection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
            }
            catch (WrongExpectedVersionException e)
                when (e.ExpectedVersion is ExpectedVersion.NoStream)
            {
                throw new DuplicateKeyException(aggregateId);
            }
        }

        private static string GetAggregateStreamName<TEventSourcedAggregate>(Guid aggregateId)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            return $"{typeof(TEventSourcedAggregate).Name}-{aggregateId}";
        }
    }
}