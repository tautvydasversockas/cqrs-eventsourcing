using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain.Common;
using Accounts.Infrastructure.Exceptions;
using EventStore.Client;

namespace Accounts.Infrastructure
{
    public sealed class EventSourcedRepository<TEventSourcedAggregate, TId> : IEventSourcedRepository<TEventSourcedAggregate, TId>
        where TEventSourcedAggregate : EventSourcedAggregate<TId>, new()
        where TId : notnull
    {
        private readonly EventStoreClient _client;

        public EventSourcedRepository(EventStoreClient client)
        {
            _client = client;
        }

        public async Task SaveAsync(TEventSourcedAggregate aggregate, CancellationToken token = default)
        {
            if (!aggregate.UncommittedEvents.Any())
                return;

            var streamName = GetStreamName(aggregate.Id);
            var originalVersion = aggregate.Version - aggregate.UncommittedEvents.Count;
            var expectedState = originalVersion is 0
                ? StreamRevision.None
                : new((ulong)originalVersion - 1);

            var metadata = new EventMetadata(MessageContext.CausationId, MessageContext.CorrelationId);
            var eventsToSave = aggregate.UncommittedEvents.Select(@event => EventStoreSerializer.Serialize(@event, metadata));

            try
            {
                await _client.AppendToStreamAsync(streamName, expectedState, eventsToSave, cancellationToken: token);
            }
            catch (WrongExpectedVersionException e) 
                when (e.ExpectedStreamRevision == StreamRevision.None)
            {
                throw new Exceptions.DuplicateKeyException(aggregate.Id);
            }

            aggregate.MarkEventsAsCommitted();
        }

        public async Task<TEventSourcedAggregate?> GetAsync(TId id, CancellationToken token = default)
        {
            var streamName = GetStreamName(id);
            var readStreamResult = _client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, cancellationToken: token);

            if (await readStreamResult.ReadState is ReadState.StreamNotFound)
                return null;

            TEventSourcedAggregate? aggregate = null;

            await foreach (var resolvedEvent in readStreamResult)
            {
                var (@event, metadata) = EventStoreSerializer.Deserialize(resolvedEvent.Event);

                if (MessageContext.MessageId is not null && MessageContext.MessageId == metadata.CausationId)
                    throw new DuplicateOperationException(MessageContext.MessageId);

                (aggregate ??= new()).ApplyEvent(@event);
            }

            return aggregate;
        }

        private static string GetStreamName(TId aggregateId)
        {
            return $"{typeof(TEventSourcedAggregate).Name.ToLower()}-{aggregateId}";
        }
    }
}