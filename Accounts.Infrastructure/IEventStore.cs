using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounts.Domain.Common;

namespace Accounts.Infrastructure
{
    public interface IEventStore
    {
        Task SaveAggregateEventsAsync<TEventSourcedAggregate>(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
            where TEventSourcedAggregate : EventSourcedAggregate;

        IAsyncEnumerable<Event> GetAggregateEventsAsync<TEventSourcedAggregate>(Guid aggregateId)
            where TEventSourcedAggregate : EventSourcedAggregate;
    }
}