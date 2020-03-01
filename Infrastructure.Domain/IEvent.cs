using System;

namespace Infrastructure.Domain
{
    public interface IEvent
    {
        Guid SourceId { get; }
    }
}