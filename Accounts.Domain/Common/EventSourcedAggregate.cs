using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ReflectionMagic;

namespace Accounts.Domain.Common
{
    public abstract class EventSourcedAggregate<TId> : Entity<TId>
    {
        private readonly List<IEvent> _events = new();
        public IReadOnlyList<IEvent> UncommittedEvents => _events;
        public void MarkEventsAsCommitted() => _events.Clear();

        public int Version { get; private set; }

        protected void Raise(IEvent @event)
        {
            _events.Add(@event);
            ApplyEvent(@event);
        }

        public void ApplyEvent(IEvent @event)
        {
            this.AsDynamic().Apply(@event);
            Version++;
        }
    }
}