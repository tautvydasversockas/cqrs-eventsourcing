using System;
using System.Threading.Tasks;
using Infrastructure.Domain;
using Infrastructure.Domain.Exceptions;

namespace Accounts.Application.Common
{
    public static class RepositoryExtensions
    {
        public static async Task CreateAsync<TEventSourcedAggregate>(this IEventSourcedRepository<TEventSourcedAggregate> repository, TEventSourcedAggregate aggregate, Guid correlationId)
            where TEventSourcedAggregate : EventSourcedAggregate
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

        public static async Task ExecuteAsync<TEventSourcedAggregate>(this IEventSourcedRepository<TEventSourcedAggregate> repository, Guid id, Action<TEventSourcedAggregate> action, Guid correlationId)
            where TEventSourcedAggregate : EventSourcedAggregate
        {
            var aggregate = await repository.GetAsync(id);
            action(aggregate);
            await repository.SaveAsync(aggregate, correlationId.ToString());
        }

        public static async Task ExecuteAsync<TEventSourcedAggregate>(this IEventSourcedRepository<TEventSourcedAggregate> repository, Guid id, Action<TEventSourcedAggregate> action, Guid correlationId, Guid operationId)
            where TEventSourcedAggregate : EventSourcedAggregate
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