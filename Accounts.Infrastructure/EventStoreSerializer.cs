using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Accounts.Domain;
using EventStore.ClientAPI;

namespace Accounts.Infrastructure
{
    public static class EventStoreSerializer
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly Type EventType = typeof(AccountOpened);
        private static Assembly EventAssembly => EventType.Assembly;
        private static string? EventNamespace => EventType.Namespace;

        public static EventData Serialize<TEvent>(TEvent @event, Metadata metadata)
            where TEvent : Event
        {
            var eventId = Guid.NewGuid();
            var eventType = @event.GetType().Name;
            var serializedEvent = Serialize(@event);
            var serializedMetadata = Serialize(metadata);
            return new(eventId, eventType, true, serializedEvent, serializedMetadata);
        }

        public static (Event @event, Metadata metadata) Deserialize(ResolvedEvent resolvedEvent)
        {
            var recordedEvent = resolvedEvent.Event;
            var eventTypeShortName = recordedEvent.EventType;
            var eventTypeFullName = EventNamespace is null
                ? eventTypeShortName
                : $"{EventNamespace}.{eventTypeShortName}";
            var eventType = EventAssembly.GetType(eventTypeFullName, true)!;
            var @event = (Event)Deserialize(recordedEvent.Data, eventType);
            var metadata = (Metadata)Deserialize(recordedEvent.Metadata, typeof(Metadata));
            return (@event, metadata);
        }

        private static byte[] Serialize<T>(T value) =>
            JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);

        private static object Deserialize(byte[] value, Type type) =>
            JsonSerializer.Deserialize(value, type, JsonOptions) ??
            throw new JsonException($"Failed to deserialize {type.Name}.");
    }
}