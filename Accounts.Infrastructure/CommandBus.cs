using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Common;
using Accounts.Domain.Common;

namespace Accounts.Infrastructure
{
    public sealed class CommandBus
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task SendAsync<TCommand>(TCommand command, CancellationToken token = default) where TCommand : Command
        {
            dynamic handler = _serviceProvider.GetService(typeof(IHandler<TCommand>));
            return handler.HandleAsync(command, token);
        }
    }
}