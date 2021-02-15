using Accounts.Domain;
using EventStore.Client;
using System;
using System.Reflection;
using System.Text.Json;

namespace Accounts.Infrastructure
{
    public static class EventStoreSerializer
    {
        private static readonly Type EventType = typeof(AccountOpened);
        private static Assembly EventAssembly => EventType.Assembly;
        private static string? EventNamespace => EventType.Namespace;

        public static EventData Serialize<TEvent>(TEvent @event, EventMetadata metadata)
            where TEvent : IEvent
        {
            var eventId = Uuid.NewUuid();
            var eventType = @event.GetType().Name;
            var serializedEvent = Serialize(@event);
            var serializedMetadata = Serialize(metadata);
            return new EventData(eventId, eventType, serializedEvent, serializedMetadata);
        }

        public static (IEvent @event, EventMetadata metadata) Deserialize(EventRecord eventRecord)
        {
            var eventTypeShortName = eventRecord.EventType;
            var eventTypeFullName = EventNamespace is null
                ? eventTypeShortName
                : $"{EventNamespace}.{eventTypeShortName}";

            var eventType = EventAssembly.GetType(eventTypeFullName) ??
                throw new ArgumentException($"Event type {eventTypeFullName} was not found.", nameof(eventRecord));

            var @event = Deserialize<IEvent>(eventRecord.Data.Span, eventType);
            var metadata = Deserialize<EventMetadata>(eventRecord.Metadata.Span);

            return (@event, metadata);
        }

        private static byte[] Serialize<T>(T obj)
            where T : notnull
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj, obj.GetType());
        }

        private static T Deserialize<T>(ReadOnlySpan<byte> utf8Json, Type? returnType = null)
            where T : notnull
        {
            returnType ??= typeof(T);
            return (T)(JsonSerializer.Deserialize(utf8Json, returnType) ??
                throw new JsonException($"Failed to deserialize {returnType.Name}."));
        }
    }
}