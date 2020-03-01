using System;
using System.Collections.Generic;

namespace Infrastructure.Domain
{
    public sealed class EventSourcedAggregateFactory
    {
        public TEventSourcedAggregate Create<TEventSourcedAggregate>(Guid id, IEnumerable<IVersionedEvent> events)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            var aggregateType = typeof(TEventSourcedAggregate);
            var constructor = aggregateType.GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) }) ??
                              throw new InvalidCastException($"Type {aggregateType.Name} must have a constructor with the following signature: .ctor(Guid, IEnumerable<IVersionedEvent>)");
            return (TEventSourcedAggregate)constructor.Invoke(new object[] { id, events });
        }
    }
}