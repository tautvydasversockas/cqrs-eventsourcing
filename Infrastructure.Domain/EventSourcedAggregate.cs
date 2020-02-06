using System.Collections.Generic;
using ReflectionMagic;

namespace Infrastructure.Domain
{
    public abstract class EventSourcedAggregate<TId> : Entity<TId>
    {
        private readonly List<IVersionedEvent<TId>> _events = new List<IVersionedEvent<TId>>();

        protected EventSourcedAggregate(TId id) 
            : base(id)
        { }

        protected EventSourcedAggregate(TId id, IEnumerable<IVersionedEvent<TId>> events)
            : this(id)
        {
            foreach (var evt in events)
                ApplyEvent(evt);
        }

        public int Version { get; private set; }

        public IReadOnlyList<IVersionedEvent<TId>> GetUncommittedEvents()
        {
            return _events;
        }

        public void MarkEventsAsCommitted()
        {
            _events.Clear();
        }

        protected void Raise(VersionedEvent<TId> evt)
        {
            evt.SourceId = Id;
            evt.Version = Version + 1;
            _events.Add(evt);
            ApplyEvent(evt);
        }

        private void ApplyEvent(IVersionedEvent<TId> evt)
        {
            this.AsDynamic().Apply(evt);
            Version = evt.Version;
        }
    }
}