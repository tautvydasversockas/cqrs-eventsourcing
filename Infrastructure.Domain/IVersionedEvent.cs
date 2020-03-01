using System;

namespace Infrastructure.Domain
{
    public interface IVersionedEvent : IEvent
    {
        Guid OperationId { get; }
        int Version { get; }
    }
}