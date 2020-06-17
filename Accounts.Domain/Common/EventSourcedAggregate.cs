using System.Collections.Generic;
using ReflectionMagic;

namespace Accounts.Domain.Common
{
    public abstract class EventSourcedAggregate : Entity
    {
        private readonly List<Event> _events = new List<Event>();

        public int Version { get; private set; }
  
        public void LoadFromHistory(IEnumerable<Event> events)
        {
            foreach (var @event in events)
                ApplyEvent(@event);
        }

        public IReadOnlyList<Event> GetUncommittedEvents()
        {
            return _events;
        }

        public void MarkEventsAsCommitted()
        {
            _events.Clear();
        }

        protected void Raise(Event @event)
        {
            @event.Version = Version + 1;
            _events.Add(@event);
            ApplyEvent(@event);
        }

        private void ApplyEvent(Event @event)
        {
            this.AsDynamic().Apply(@event);
            Version++;
        }
    }
}