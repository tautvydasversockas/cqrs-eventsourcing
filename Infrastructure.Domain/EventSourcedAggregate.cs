using System;
using System.Collections.Generic;
using ReflectionMagic;

namespace Infrastructure.Domain
{
    public abstract class EventSourcedAggregate : Entity
    {
        private readonly HashSet<Guid> _operationIds = new HashSet<Guid>();
        private readonly List<IVersionedEvent> _events = new List<IVersionedEvent>();
        private Guid _operationId = Guid.NewGuid();

        protected EventSourcedAggregate(Guid id)
            : base(id) { }

        protected EventSourcedAggregate(Guid id, IEnumerable<IVersionedEvent> events)
            : this(id)
        {
            foreach (var evt in events)
                ApplyEvent(evt);
        }

        public int Version { get; private set; }

        public IReadOnlyList<IVersionedEvent> GetUncommittedEvents()
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

        protected void Raise(VersionedEvent @event)
        {
            @event.OperationId = _operationId;
            @event.SourceId = Id;
            @event.Version = Version + 1;
            _events.Add(@event);
            ApplyEvent(@event);
        }

        private void ApplyEvent(IVersionedEvent @event)
        {
            this.AsDynamic().Apply(@event);
            Version = @event.Version;
            if (!_operationIds.Contains(@event.OperationId))
                _operationIds.Add(@event.OperationId);
        }
    }
}