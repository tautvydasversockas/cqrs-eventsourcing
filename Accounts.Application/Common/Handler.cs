using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Exceptions;
using Accounts.Domain.Common;

namespace Accounts.Application.Common
{
    public abstract class Handler<TEventSourcedAggregate, TId>
        where TEventSourcedAggregate : EventSourcedAggregate<TId>, new()
        where TId : notnull
    {
        private readonly IEventSourcedRepository<TEventSourcedAggregate, TId> _repository;

        protected Handler(IEventSourcedRepository<TEventSourcedAggregate, TId> repository)
        {
            _repository = repository;
        }

        protected async Task CreateAsync(TEventSourcedAggregate aggregate, CancellationToken token)
        {
            await _repository.SaveAsync(aggregate, token);
        }

        protected async Task UpdateAsync(TId id, Action<TEventSourcedAggregate> action, CancellationToken token)
        {
            var aggregate = await _repository.GetAsync(id, token) ??
                throw new EntityNotFoundException(typeof(TEventSourcedAggregate).Name, id);

            action(aggregate);

            await _repository.SaveAsync(aggregate, token);
        }
    }
}