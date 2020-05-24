using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using Infrastructure.Domain;
using Infrastructure.Domain.Exceptions;

namespace Infrastructure
{
    public sealed class EventStoreRepository<TEventSourcedAggregate> : IEventSourcedRepository<TEventSourcedAggregate>
        where TEventSourcedAggregate : EventSourcedAggregate
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreSerializer _serializer;
        private readonly EventSourcedAggregateFactory _aggregateFactory;

        public EventStoreRepository(
            IEventStoreConnection connection,
            EventStoreSerializer serializer,
            EventSourcedAggregateFactory aggregateFactory)
        {
            _connection = connection;
            _serializer = serializer;
            _aggregateFactory = aggregateFactory;
        }

        public async Task SaveAsync(TEventSourcedAggregate aggregate, Guid correlationId)
        {
            var events = aggregate.GetUncommittedEvents();
            if (!events.Any())
                return;

            var streamName = GetStreamName(aggregate.Id);
            var originalVersion = aggregate.Version - events.Count;
            var expectedVersion = originalVersion == 0
                ? ExpectedVersion.NoStream
                : originalVersion - 1;
            var commitMetadata = new Dictionary<string, object>
            {
                [EventStoreMetadataKeys.CorrelationId] = correlationId
            };
            var eventsToSave = events.Select(e => _serializer.Serialize(e, commitMetadata));

            try
            {
                await _connection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
            }
            catch (WrongExpectedVersionException e) when (e.ExpectedVersion == ExpectedVersion.NoStream)
            {
                throw new DuplicateKeyException(aggregate.Id);
            }

            aggregate.MarkEventsAsCommitted();
        }

        public async Task<TEventSourcedAggregate> GetAsync(Guid id)
        {
            var events = new List<IVersionedEvent>();
            StreamEventsSlice currentSlice;
            long nextSliceStart = StreamPosition.Start;
            var streamName = GetStreamName(id);

            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 10, false);
                nextSliceStart = currentSlice.NextEventNumber;
                events.AddRange(currentSlice.Events.Select(e => (IVersionedEvent)_serializer.Deserialize(e).Item1));
            } while (!currentSlice.IsEndOfStream);

            if (!events.Any())
                throw new EntityNotFoundException(typeof(TEventSourcedAggregate).Name, id);

            return _aggregateFactory.Create<TEventSourcedAggregate>(id, events);
        }

        private static string GetStreamName(Guid id)
        {
            return $"{typeof(TEventSourcedAggregate).Name}-{id}";
        }
    }
}