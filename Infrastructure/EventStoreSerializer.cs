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

        public EventData Serialize<TEvent>(TEvent evt, IDictionary<string, object> evtMetadata) where TEvent : IEvent
        {
            var evtId = Guid.NewGuid();
            var type = evt.GetType().Name;
            var data = Serialize(evt);
            var metadata = Serialize(evtMetadata);
            return new EventData(evtId, type, true, data, metadata);
        }

        public (object, IDictionary<string, object>) Deserialize(ResolvedEvent resolvedEvt)
        {
            var recordedEvt = resolvedEvt.Event;
            var metadata = (Dictionary<string, object>?)Deserialize(recordedEvt.Metadata, typeof(Dictionary<string, object>)) ??
                throw new InvalidOperationException("Cannot deserialize event metadata");
            var evtTypeName = _eventNamespace == null
                ? recordedEvt.EventType :
                $"{_eventNamespace}.{recordedEvt.EventType}";
            var evtType = _eventAssembly.GetType(evtTypeName) ??
                throw new InvalidOperationException($"Cannot find event type '{evtTypeName}'");
            var evt = Deserialize(recordedEvt.Data, evtType) ??
                throw new InvalidOperationException($"Cannot deserialize event '{evtTypeName}'");
            return (evt, metadata);
        }

        private byte[] Serialize(object value) =>
            _encoding.GetBytes(JsonConvert.SerializeObject(value, _stringEnumConverter));

        private object? Deserialize(byte[] value, Type type) =>
            JsonConvert.DeserializeObject(_encoding.GetString(value), type, _stringEnumConverter);
    }
}