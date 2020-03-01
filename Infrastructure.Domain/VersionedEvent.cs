using System;

namespace Infrastructure.Domain
{
    public abstract class VersionedEvent : IVersionedEvent
    {
        public Guid OperationId { get; set; }
        public Guid SourceId { get; set; }
        public int Version { get; set; }
    }
}