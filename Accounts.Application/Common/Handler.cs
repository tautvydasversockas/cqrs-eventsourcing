using System;
using System.Threading.Tasks;
using Accounts.Application.Common.Exceptions;
using Accounts.Domain.Common;

namespace Accounts.Application.Common
{
    public abstract class Handler<TEventSourcedAggregate> where TEventSourcedAggregate : EventSourcedAggregate, new()
    {
        private readonly IEventSourcedRepository<TEventSourcedAggregate> _repository;

        protected Handler(IEventSourcedRepository<TEventSourcedAggregate> repository)
        {
            _repository = repository;
        }

        protected async Task CreateAsync(TEventSourcedAggregate aggregate)
        {
            await _repository.SaveAsync(aggregate);
        }

        protected async Task UpdateAsync(Guid id, Action<TEventSourcedAggregate> action)
        {
            var aggregate = await _repository.GetAsync(id) ??
                throw new EntityNotFoundException(nameof(TEventSourcedAggregate), id);

            action(aggregate);

            await _repository.SaveAsync(aggregate);
        }
    }
}