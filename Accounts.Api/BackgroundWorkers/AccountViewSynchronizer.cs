using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain.Events;
using Accounts.Infrastructure;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Accounts.Api.BackgroundWorkers
{
    public sealed class AccountViewSynchronizer : BackgroundService
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreSerializer _serializer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private EventStorePersistentSubscriptionBase? _subscription;
        private CancellationToken _token;

        public AccountViewSynchronizer(IEventStoreConnection connection, EventStoreSerializer serializer, IServiceScopeFactory serviceScopeFactory)
        {
            _connection = connection;
            _serializer = serializer;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            _token = token;
            return SubscribeAsync();
        }

        private async Task SubscribeAsync()
        {
            const string stream = "$ce-Account";
            const string groupName = "Account-View";
            _subscription = await _connection.ConnectToPersistentSubscriptionAsync(stream, groupName, EventAppeared, SubscriptionDropped);
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
        {
            if (IsSystemEvent(resolvedEvent))
                return;

            var (@event, _) = _serializer.Deserialize(resolvedEvent);

            switch (@event)
            {
                case AccountOpened e:
                    await UpdateViewAsync(view => view.HandleAsync(e, _token));
                    break;

                case DepositedToAccount e:
                    await UpdateViewAsync(view => view.HandleAsync(e, _token));
                    break;

                case WithdrawnFromAccount e:
                    await UpdateViewAsync(view => view.HandleAsync(e, _token));
                    break;

                case AddedInterestsToAccount e:
                    await UpdateViewAsync(view => view.HandleAsync(e, _token));
                    break;

                case AccountFrozen e:
                    await UpdateViewAsync(view => view.HandleAsync(e, _token));
                    break;

                case AccountUnfrozen e:
                    await UpdateViewAsync(view => view.HandleAsync(e, _token));
                    break;
            }
        }

        private static bool IsSystemEvent(ResolvedEvent resolvedEvent)
        {
            var recordedEvent = resolvedEvent.Event;
            return !recordedEvent.Data.Any() || !recordedEvent.IsJson || recordedEvent.EventType.StartsWith('$');
        }

        private async Task UpdateViewAsync(Func<AccountView, Task> handleAsync)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var accountView = scope.ServiceProvider.GetService<AccountView>();
            await handleAsync(accountView);
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase subscription, SubscriptionDropReason reason, Exception e)
        {
            SubscribeAsync().GetAwaiter().GetResult();
        }

        public override Task StopAsync(CancellationToken token)
        {
            _subscription?.Stop(TimeSpan.FromSeconds(5));
            return base.StopAsync(token);
        }
    }
}