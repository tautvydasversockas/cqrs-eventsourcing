using System;

namespace Infrastructure.Domain
{
    public abstract class VersionedEvent<TSourceId> : IVersionedEvent<TSourceId>
    {
        public Guid OperationId { get; set; }
        public TSourceId SourceId { get; set; }
        public int Version { get; set; }
    }
}