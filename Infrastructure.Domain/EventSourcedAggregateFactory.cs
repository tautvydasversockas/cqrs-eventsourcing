using System;
using System.Collections.Generic;

namespace Infrastructure.Domain
{
    public sealed class EventSourcedAggregateFactory
    {
        public TEventSourcedAggregate Create<TEventSourcedAggregate, TId>(TId id, IEnumerable<IVersionedEvent<TId>> events)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
        {
            var aggregateType = typeof(TEventSourcedAggregate);
            var idType = typeof(TId);

            var constructor = aggregateType.GetConstructor(new[] { idType, typeof(IEnumerable<IVersionedEvent<TId>>) }) ??
                              throw new InvalidCastException($"Type {aggregateType.Name} must have a constructor with the following signature: .ctor({idType.Name}, IEnumerable<IVersionedEvent<{idType.Name}>>)");

            return (TEventSourcedAggregate)constructor.Invoke(new object[] { id, events });
        }
    }
}