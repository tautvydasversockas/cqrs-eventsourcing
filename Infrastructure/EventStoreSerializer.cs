using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure
{
    public class EventStoreSerializer
    {
        private readonly Assembly _eventAssembly;
        private readonly string _eventNamespace;
        private readonly Encoding _encoding;
        private readonly StringEnumConverter _stringEnumConverter;

        public EventStoreSerializer(Assembly eventAssembly, string eventNamespace)
        {
            _eventAssembly = eventAssembly;
            _eventNamespace = eventNamespace;
            _encoding = Encoding.UTF8;
            _stringEnumConverter = new StringEnumConverter();
        }

        public EventData Serialize(object evt, IDictionary<string, object> evtMetadata)
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
            var metadata = Deserialize<Dictionary<string, object>>(recordedEvt.Metadata);
            var evtType = _eventAssembly.GetType($"{_eventNamespace}.{recordedEvt.EventType}");
            var evt = Deserialize(recordedEvt.Data, evtType);
            return (evt, metadata);
        }

        private byte[] Serialize<T>(T value) =>
            _encoding.GetBytes(JsonConvert.SerializeObject(value, _stringEnumConverter));

        private T Deserialize<T>(byte[] value) =>
            (T)Deserialize(value, typeof(T));

        private object Deserialize(byte[] value, Type type) =>
            JsonConvert.DeserializeObject(_encoding.GetString(value), type, _stringEnumConverter);
    }
}