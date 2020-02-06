namespace Infrastructure.Domain
{
    public abstract class VersionedEvent<TSourceId> : IVersionedEvent<TSourceId>
    {
        public TSourceId SourceId { get; set; }
        public int Version { get; set; }
    }
}