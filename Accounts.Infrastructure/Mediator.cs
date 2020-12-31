using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Common;
using Accounts.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Infrastructure
{
    public sealed class Mediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task SendAsync<TCommand>(TCommand command, CancellationToken token = default) 
            where TCommand : ICommand
        {
            var handler = _serviceProvider.GetRequiredService<IHandler<TCommand>>();
            return handler.HandleAsync(command, token);
        }
    }
}