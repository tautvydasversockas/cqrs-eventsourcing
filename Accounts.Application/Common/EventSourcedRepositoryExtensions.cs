using Accounts.Application.Common.Exceptions;
using Accounts.Domain.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Application.Common
{
    public static class EventSourcedRepositoryExtensions
    {
        public static async Task UpdateAsync<TEventSourcedAggregate, TId>(
            this IEventSourcedRepository<TEventSourcedAggregate, TId> repository,
            TId id,
            Action<TEventSourcedAggregate> action,
            CancellationToken token = default)
            where TEventSourcedAggregate : EventSourcedAggregate<TId>
            where TId : notnull
        {
            var aggregate = await repository.GetAsync(id, token) ??
                throw new EntityNotFoundException(typeof(TEventSourcedAggregate).Name, id);

            action(aggregate);

            await repository.SaveAsync(aggregate, token);
        }
    }
}