using System;
using System.Collections.Generic;
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
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreSerializer _serializer;
        private readonly MessageContext _context;

        public EventSourcedRepository(IEventStoreConnection connection, EventStoreSerializer serializer, MessageContext context)
        {
            _connection = connection;
            _serializer = serializer;
            _context = context;
        }

        public async Task SaveAsync(TEventSourcedAggregate aggregate)
        {
            var events = aggregate.GetUncommittedEvents();
            if (!events.Any())
                return;

            var streamName = GetStreamName(aggregate.Id);
            var originalVersion = aggregate.Version - events.Count;
            var expectedVersion = originalVersion == 0
                ? ExpectedVersion.NoStream
                : originalVersion - 1;
            var metadata = new Metadata(_context.CausationId, _context.CorrelationId);
            var eventsToSave = events.Select(@event => _serializer.Serialize(@event, metadata));

            try
            {
                await _connection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
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
            var events = new List<Event>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = (long)StreamPosition.Start;
            var streamName = GetStreamName(id);

            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 10, false);
                nextSliceStart = currentSlice.NextEventNumber;
                events.AddRange(currentSlice.Events.Select(resolvedEvent =>
                {
                    var (@event, metadata) = _serializer.Deserialize(resolvedEvent);

                    if (_context.Id == metadata.CausationId)
                        throw new DuplicateOperationException(_context.CausationId);

                    return @event;
                }));
            } while (!currentSlice.IsEndOfStream);

            if (!events.Any())
                return null;

            var aggregate = new TEventSourcedAggregate();
            aggregate.LoadFromHistory(events);
            return aggregate;
        }

        private static string GetStreamName(Guid id)
        {
            return $"{typeof(TEventSourcedAggregate).Name}-{id}";
        }
    }
}