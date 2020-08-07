using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Common;
using Accounts.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Infrastructure
{
    public sealed class MessageBus
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task SendAsync<TCommand>(TCommand command, MessageContext context, CancellationToken token = default) 
            where TCommand : Command
        {
            var contextProvider = _serviceProvider.GetRequiredService<MessageContextProvider>();
            contextProvider.Context = context;

            var handler = _serviceProvider.GetRequiredService<IHandler<TCommand>>();
            return handler.HandleAsync(command, token);
        }
    }
}