namespace Accounts.Infrastructure;

public static class EventStoreSerializer
{
    private static readonly Type EventType = typeof(AccountOpened);
    private static readonly Assembly EventAssembly = EventType.Assembly;
    private static readonly string EventTypeNamespacePrefix =
        EventType.Namespace is null ? string.Empty : $"{EventType.Namespace}.";

    public static EventData Serialize<TEvent>(TEvent @event, EventMetadata metadata)
        where TEvent : IEvent
    {
        var eventId = Uuid.NewUuid();
        var eventType = @event.GetType().Name;
        var serializedEvent = Serialize(@event);
        var serializedMetadata = Serialize(metadata);
        return new EventData(eventId, eventType, serializedEvent, serializedMetadata);
    }

    public static IEvent DeserializeEvent(EventRecord eventRecord)
    {
        var eventTypeName = $"{EventTypeNamespacePrefix}{eventRecord.EventType}";
        var eventType = EventAssembly.GetType(eventTypeName, throwOnError: true)!;
        return (IEvent)Deserialize(eventRecord.Data.Span, eventType);
    }

    public static EventMetadata DeserializeMetadata(EventRecord eventRecord)
    {
        return Deserialize<EventMetadata>(eventRecord.Metadata.Span);
    }

    private static byte[] Serialize<T>(T obj)
        where T : notnull
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, obj.GetType());
    }

    private static T Deserialize<T>(ReadOnlySpan<byte> utf8Json)
    {
        return (T)Deserialize(utf8Json, typeof(T));
    }

    private static object Deserialize(ReadOnlySpan<byte> utf8Json, Type returnType)
    {
        return JsonSerializer.Deserialize(utf8Json, returnType) ??
            throw new JsonException($"Failed to deserialize {returnType.Name}.");
    }
}
