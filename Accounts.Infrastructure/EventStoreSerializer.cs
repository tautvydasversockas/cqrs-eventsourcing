using System;
using System.Reflection;
using System.Text.Json;
using Accounts.Domain;
using EventStore.Client;

namespace Accounts.Infrastructure
{
    public static class EventStoreSerializer
    {
        private static readonly Type EventType = typeof(AccountOpened);
        private static Assembly EventAssembly => EventType.Assembly;
        private static string? EventNamespace => EventType.Namespace;

        public static EventData Serialize<TEvent>(TEvent @event, Metadata metadata)
            where TEvent : Event
        {
            var eventId = Uuid.NewUuid();
            var eventType = @event.GetType().Name;
            var serializedEvent = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());
            var serializedMetadata = JsonSerializer.SerializeToUtf8Bytes(metadata, metadata.GetType());
            return new(eventId, eventType, serializedEvent, serializedMetadata);
        }

        public static (Event @event, Metadata metadata) Deserialize(EventRecord eventRecord)
        {
            var eventTypeShortName = eventRecord.EventType;
            var eventTypeFullName = EventNamespace is null
                ? eventTypeShortName
                : $"{EventNamespace}.{eventTypeShortName}";

            var eventType = EventAssembly.GetType(eventTypeFullName) ??
                throw new ArgumentException($"Event type {eventTypeFullName} was not found.", nameof(eventRecord));

            var @event = (Event)(JsonSerializer.Deserialize(eventRecord.Data.Span, eventType) ??
                throw new JsonException($"Failed to deserialize {eventType.Name}."));

            var metadata = (Metadata)(JsonSerializer.Deserialize(eventRecord.Metadata.Span, typeof(Metadata)) ??
                throw new JsonException($"Failed to deserialize {nameof(Metadata)}."));

            return (@event, metadata);
        }
    }
}