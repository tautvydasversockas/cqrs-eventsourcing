using System;
using System.Threading.Tasks;
using Infrastructure.Domain;
using Infrastructure.Domain.Exceptions;

namespace Accounts.Application.Common
{
    public static class RepositoryExtensions
    {
        public static async Task CreateAsync<TEventSourcedAggregate, TId>(
            this IEventSourcedRepository<TEventSourcedAggregate, TId> repository, TEventSourcedAggregate aggregate, Guid correlationId)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
        {
            try
            {
                await repository.SaveAsync(aggregate, correlationId.ToString());
            }
            catch (DuplicateKeyException)
            {
                //Ignore idempotent operation
            }
        }

        public static async Task ExecuteAsync<TEventSourcedAggregate, TId>(
            this IEventSourcedRepository<TEventSourcedAggregate, TId> repository, TId id, Action<TEventSourcedAggregate> action, Guid correlationId)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
        {
            var aggregate = await repository.GetAsync(id);
            action(aggregate);
            await repository.SaveAsync(aggregate, correlationId.ToString());
        }

        public static async Task ExecuteAsync<TEventSourcedAggregate, TId>(
            this IEventSourcedRepository<TEventSourcedAggregate, TId> repository, TId id, Action<TEventSourcedAggregate> action, Guid correlationId, Guid operationId)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
        {
            var aggregate = await repository.GetAsync(id);
            try
            {
                aggregate.SetOperation(operationId);
            }
            catch (InvalidOperationException)
            {
                //Ignore idempotent operation
                return;
            }
            action(aggregate);
            await repository.SaveAsync(aggregate, correlationId.ToString());
        }
    }
}