namespace Accounts.Infrastructure;

public sealed class EventSourcedRepository<TEventSourcedAggregate, TId> : IEventSourcedRepository<TEventSourcedAggregate, TId>
    where TEventSourcedAggregate : EventSourcedAggregate<TId>
    where TId : notnull
{
    private readonly EventStoreClient _client;

    public EventSourcedRepository(EventStoreClient client)
    {
        _client = client;
    }

    public async Task SaveAsync(TEventSourcedAggregate aggregate, CancellationToken token = default)
    {
        if (aggregate.UncommittedEvents.Count is 0)
            return;

        var streamName = GetStreamName(aggregate.Id);

        var originalAggregateVersion = aggregate.Version - aggregate.UncommittedEvents.Count;
        var expectedStreamRevision = originalAggregateVersion is 0
            ? StreamRevision.None
            : new StreamRevision((ulong)originalAggregateVersion - 1);

        var metadata = new EventMetadata(
            CausationId: RequestContext.RequestId,
            CorrelationId: RequestContext.CorrelationId);

        var eventsToSave = aggregate.UncommittedEvents.Select(@event =>
            EventStoreSerializer.Serialize(@event, metadata));

        try
        {
            await _client.AppendToStreamAsync(
                streamName, expectedStreamRevision, eventsToSave, cancellationToken: token);
        }
        catch (WrongExpectedVersionException e)
            when (e.ExpectedStreamRevision == StreamRevision.None)
        {
            throw new DuplicateKeyException(aggregate.Id);
        }

        aggregate.MarkEventsAsCommitted();
    }

    public async Task<TEventSourcedAggregate?> GetAsync(TId id, CancellationToken token = default)
    {
        var streamName = GetStreamName(id);
        var readStreamResult = _client.ReadStreamAsync(
            Direction.Forwards, streamName, StreamPosition.Start, cancellationToken: token);

        if (await readStreamResult.ReadState is ReadState.StreamNotFound)
            return null;

        var aggregate = (TEventSourcedAggregate)Activator.CreateInstance(
            typeof(TEventSourcedAggregate), nonPublic: true)!;

        await foreach (var resolvedEvent in readStreamResult)
        {
            if (RequestContext.RequestId is not null)
            {
                var metadata = EventStoreSerializer.DeserializeMetadata(resolvedEvent.Event);
                if (RequestContext.RequestId == metadata.CausationId)
                    throw new DuplicateRequestException(RequestContext.RequestId);
            }

            var @event = EventStoreSerializer.DeserializeEvent(resolvedEvent.Event);
            aggregate.ApplyEvent(@event);
        }

        return aggregate;
    }

    private static string GetStreamName(TId aggregateId)
    {
        return $"{typeof(TEventSourcedAggregate).Name.ToLower()}-{aggregateId}";
    }
}
