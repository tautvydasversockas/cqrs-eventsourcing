using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure
{
    public static class EventStoreSerializer
    {
        private static readonly Encoding Encoding = Encoding.UTF8;
        private static readonly StringEnumConverter StringEnumConverter = new StringEnumConverter();

        public static byte[] Serialize<T>(T value) =>
            Encoding.GetBytes(JsonConvert.SerializeObject(value, StringEnumConverter));

        public static T Deserialize<T>(byte[] value) =>
            JsonConvert.DeserializeObject<T>(Encoding.GetString(value), StringEnumConverter);

        public static object Deserialize(byte[] value, Type type) =>
            JsonConvert.DeserializeObject(Encoding.GetString(value), type, StringEnumConverter);
    }
}