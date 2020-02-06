namespace Infrastructure
{
    public static class EventStoreMetadataKeys
    {
        public const string CorrelationId = "$correlationId";
        public const string AggregateClrType = "AggregateClrType";
        public const string EventClrType = "EventClrType";
    }
}