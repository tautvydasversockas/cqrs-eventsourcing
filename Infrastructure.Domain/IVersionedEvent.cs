using System;

namespace Infrastructure.Domain
{
    public interface IVersionedEvent<out TSourceId> : IEvent<TSourceId>
    {
        Guid OperationId { get; }
        int Version { get; }
    }
}