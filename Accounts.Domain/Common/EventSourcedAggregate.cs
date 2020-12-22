using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ReflectionMagic;

namespace Accounts.Domain.Common
{
    public abstract class EventSourcedAggregate<TId> : Entity<TId>
        where TId : notnull
    {
        private readonly List<Event> _events = new();
        public IReadOnlyList<Event> UncommittedEvents => _events;
        public void MarkEventsAsCommitted() => _events.Clear();

        public int Version { get; private set; }

        protected void Raise(Event @event)
        {
            @event.Version = Version + 1;
            _events.Add(@event);
            ApplyEvent(@event);
        }

        public void ApplyEvent(Event @event)
        {
            this.AsDynamic().Apply(@event);
            Version++;
        }
    }
}