using System;
using System.Reflection;
using System.Text;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Accounts.Infrastructure
{
    public static class EventStoreSerializer
    {
        private static readonly Encoding Encoding = Encoding.UTF8;
        private static readonly StringEnumConverter StringEnumConverter = new StringEnumConverter();
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
            return new EventData(eventId, eventType, true, serializedEvent, serializedMetadata);
        }

        public static (Event, Metadata) Deserialize(ResolvedEvent resolvedEvent)
        {
            var recordedEvent = resolvedEvent.Event;
            var eventTypeShortName = recordedEvent.EventType;
            var eventTypeFullName = EventNamespace == null
                ? eventTypeShortName
                : $"{EventNamespace}.{eventTypeShortName}";
            var eventType = EventAssembly.GetType(eventTypeFullName, true);
            var @event = (Event)Deserialize(recordedEvent.Data, eventType);
            var metadata = (Metadata)Deserialize(recordedEvent.Metadata, typeof(Metadata));
            return (@event, metadata);
        }

        private static byte[] Serialize(object value)
        {
            var json = JsonConvert.SerializeObject(value, StringEnumConverter);
            return Encoding.GetBytes(json);
        }

        private static object Deserialize(byte[] value, Type type)
        {
            var json = Encoding.GetString(value);
            return JsonConvert.DeserializeObject(json, type, StringEnumConverter) ??
                throw new JsonSerializationException($"Failed to deserialize {type.Name}.");
        }
    }
}