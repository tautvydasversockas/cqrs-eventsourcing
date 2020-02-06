using System;
using System.Threading.Tasks;
using Accounts.Application.Common.Exceptions;
using Infrastructure.Domain;

namespace Accounts.Application.Common
{
    public static class EventSourcedRepositoryExtensions
    {
        public static async Task AddAsync<TEventSourcedAggregate, TId>(
            this IEventSourcedRepository<TEventSourcedAggregate, TId> repository, TEventSourcedAggregate aggregate)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
        {
            await repository.SaveAsync(aggregate, Guid.NewGuid().ToString());
        }

        public static async Task ExecuteAsync<TEventSourcedAggregate, TId>(
            this IEventSourcedRepository<TEventSourcedAggregate, TId> repository, TId id, Action<TEventSourcedAggregate> action)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
        {
            await repository.ExecuteAsync(id, action, Guid.NewGuid().ToString());
        }

        public static async Task ExecuteAsync<TEventSourcedAggregate, TId>(
            this IEventSourcedRepository<TEventSourcedAggregate, TId> repository, TId id, Action<TEventSourcedAggregate> action, string correlationId)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
        {
            var aggregate = await repository.GetAsync(id) ??
                            throw new EntityNotFoundException(typeof(TEventSourcedAggregate).Name, id);
            action(aggregate);
            await repository.SaveAsync(aggregate, correlationId);
        }
    }
}