using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EventStore.ClientAPI;
using Infrastructure.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure
{
    public class EventStoreSerializer
    {
        private readonly Assembly _eventAssembly;
        private readonly string? _eventNamespace;
        private readonly Encoding _encoding;
        private readonly StringEnumConverter _stringEnumConverter;

        public EventStoreSerializer(Assembly eventAssembly, string? eventNamespace)
        {
            _eventAssembly = eventAssembly;
            _eventNamespace = eventNamespace;
            _encoding = Encoding.UTF8;
            _stringEnumConverter = new StringEnumConverter();
        }

        public EventData Serialize<TEvent>(TEvent @event, IDictionary<string, object> evtMetadata) where TEvent : IEvent
        {
            var eventId = Guid.NewGuid();
            var type = @event.GetType().Name;
            var data = Serialize(@event);
            var metadata = Serialize(evtMetadata);
            return new EventData(eventId, type, true, data, metadata);
        }

        public (object, IDictionary<string, object>) Deserialize(ResolvedEvent resolvedEvent)
        {
            var recordedEvent = resolvedEvent.Event;
            var metadata = (Dictionary<string, object>?)Deserialize(recordedEvent.Metadata, typeof(Dictionary<string, object>)) ??
                throw new InvalidOperationException("Cannot deserialize event metadata.");
            var eventTypeName = _eventNamespace == null
                ? recordedEvent.EventType
                : $"{_eventNamespace}.{recordedEvent.EventType}";
            var eventType = _eventAssembly.GetType(eventTypeName) ??
                throw new InvalidOperationException($"Cannot find event type '{eventTypeName}'.");
            var @event = Deserialize(recordedEvent.Data, eventType) ??
                throw new InvalidOperationException($"Cannot deserialize event '{eventTypeName}'.");
            return (@event, metadata);
        }

        private byte[] Serialize(object value) =>
            _encoding.GetBytes(JsonConvert.SerializeObject(value, _stringEnumConverter));

        private object? Deserialize(byte[] value, Type type) =>
            JsonConvert.DeserializeObject(_encoding.GetString(value), type, _stringEnumConverter);
    }
}
