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
    public sealed class EventStoreRepository<TEventSourcedAggregate, TId> : IEventSourcedRepository<TEventSourcedAggregate, TId>
        where TEventSourcedAggregate : EventSourcedAggregate<TId>
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventSourcedAggregateFactory _aggregateFactory;

        public EventStoreRepository(IEventStoreConnection connection, EventSourcedAggregateFactory aggregateFactory)
        {
            _connection = connection;
            _aggregateFactory = aggregateFactory;
        }

        public async Task SaveAsync(TEventSourcedAggregate aggregate, string correlationId)
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
                {EventStoreMetadataKeys.CorrelationId, correlationId},
                {EventStoreMetadataKeys.AggregateClrType, aggregate.GetType().AssemblyQualifiedName}
            };
            var eventsToSave = events.Select(e =>
            {
                var evtId = Guid.NewGuid();
                var evtType = e.GetType();
                var serializedEvt = EventStoreSerializer.Serialize(e);
                var metadata = new Dictionary<string, object>(commitMetadata)
                {
                    { EventStoreMetadataKeys.EventClrType, evtType.AssemblyQualifiedName}
                };
                var serializedMetadata = EventStoreSerializer.Serialize(metadata);
                return new EventData(evtId, evtType.Name, true, serializedEvt, serializedMetadata);
            });

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

        public async Task<TEventSourcedAggregate> GetAsync(TId id)
        {
            var events = new List<IVersionedEvent<TId>>();
            StreamEventsSlice currentSlice;
            long nextSliceStart = StreamPosition.Start;
            var streamName = GetStreamName(id);

            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, count: 10, resolveLinkTos: false);
                nextSliceStart = currentSlice.NextEventNumber;
                events.AddRange(currentSlice.Events.Select(e =>
                {
                    var recordedEvt = e.Event;
                    var metadata = EventStoreSerializer.Deserialize<Dictionary<string, object>>(recordedEvt.Metadata);
                    var evtType = Type.GetType(metadata[EventStoreMetadataKeys.EventClrType].ToString());
                    return (IVersionedEvent<TId>)EventStoreSerializer.Deserialize(recordedEvt.Data, evtType);
                }));
            } while (!currentSlice.IsEndOfStream);

            if (!events.Any())
                throw new EntityNotFoundException(typeof(TEventSourcedAggregate).Name, id);

            return _aggregateFactory.Create<TEventSourcedAggregate, TId>(id, events);
        }

        private static string GetStreamName(TId id)
        {
            return $"{typeof(TEventSourcedAggregate).Name}-{id.ToString()}";
        }
    }
}