using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Api.HealthChecks;
using Accounts.Domain.Events;
using Accounts.Infrastructure;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Accounts.ReadModelSynchronizer
{
    public sealed class AccountViewSynchronizer : BackgroundService
    {
        private const string Stream = "$ce-Account";
        private const string GroupName = "Account-View";

        private readonly IEventStoreConnection _connection;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BackgroundServiceHealthCheck _healthCheck;

        private EventStorePersistentSubscriptionBase? _subscription;
        private CancellationToken _token;

        public AccountViewSynchronizer(
            IEventStoreConnection connection, 
            IServiceScopeFactory serviceScopeFactory,
            BackgroundServiceHealthCheck healthCheck)
        {
            _connection = connection;
            _serviceScopeFactory = serviceScopeFactory;
            _healthCheck = healthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _token = token;
            await _connection.ConnectAsync();
            await SubscribeAsync();
        }

        private async Task SubscribeAsync()
        {
            _subscription = await _connection.ConnectToPersistentSubscriptionAsync(Stream, GroupName, EventAppeared, SubscriptionDropped);
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
        {
            if (IsSystemEvent(resolvedEvent))
                return;

            var (@event, _) = EventStoreSerializer.Deserialize(resolvedEvent);

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

            _healthCheck.SetLastProcessTime();
        }

        private static bool IsSystemEvent(ResolvedEvent resolvedEvent)
        {
            var recordedEvent = resolvedEvent.Event;
            return !recordedEvent.Data.Any() || !recordedEvent.IsJson || recordedEvent.EventType.StartsWith('$');
        }

        private async Task UpdateViewAsync(Func<AccountView, Task> handleAsync)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var accountView = scope.ServiceProvider.GetRequiredService<AccountView>();
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