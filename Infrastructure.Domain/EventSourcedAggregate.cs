using System;
using System.Collections.Generic;
using ReflectionMagic;

namespace Infrastructure.Domain
{
    public abstract class EventSourcedAggregate<TId> : Entity<TId>
    {
        private readonly HashSet<Guid> _operationIds = new HashSet<Guid>();
        private readonly List<IVersionedEvent<TId>> _events = new List<IVersionedEvent<TId>>();
        private Guid _operationId = Guid.NewGuid();

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

        public void SetOperation(Guid operationId)
        {
            if (_operationIds.Contains(operationId))
                throw new InvalidOperationException("Duplicate Operation.");

            _operationId = operationId;
        }

        protected void Raise(VersionedEvent<TId> evt)
        {
            evt.OperationId = _operationId;
            evt.SourceId = Id;
            evt.Version = Version + 1;
            _events.Add(evt);
            ApplyEvent(evt);
        }

        private void ApplyEvent(IVersionedEvent<TId> evt)
        {
            this.AsDynamic().Apply(evt);
            Version = evt.Version;
            if (!_operationIds.Contains(evt.OperationId))
                _operationIds.Add(evt.OperationId);
        }
    }
}