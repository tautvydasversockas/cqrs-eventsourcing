using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain;
using Accounts.Infrastructure;
using Accounts.Infrastructure.HealthChecks;
using Accounts.ReadModel;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Accounts.ReadModelSynchronizer
{
    public sealed class App : BackgroundService
    {
        private readonly EventStorePersistentSubscriptionsClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BackgroundServiceHealthCheck _healthCheck;

        private PersistentSubscription? _subscription;

        public App(
            EventStorePersistentSubscriptionsClient client,
            IServiceScopeFactory serviceScopeFactory,
            BackgroundServiceHealthCheck healthCheck)
        {
            _client = client;
            _serviceScopeFactory = serviceScopeFactory;
            _healthCheck = healthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            await SubscribeAsync(token);
        }

        private async Task SubscribeAsync(CancellationToken token)
        {
            _subscription = await _client.SubscribeAsync(
                streamName: "$ce-account",
                groupName: "account-view",
                eventAppeared: async (subscription, resolvedEvent, retryCount, token) =>
                {
                    if (IsSystemEvent(resolvedEvent))
                        return;

                    var (@event, _) = EventStoreSerializer.Deserialize(resolvedEvent.Event);

                    switch (@event)
                    {
                        case AccountOpened accountOpened:
                            await UpdateViewAsync(view => view.HandleAsync(accountOpened, token));
                            break;

                        case DepositedToAccount depositedToAccount:
                            await UpdateViewAsync(view => view.HandleAsync(depositedToAccount, token));
                            break;

                        case WithdrawnFromAccount withdrawnFromAccount:
                            await UpdateViewAsync(view => view.HandleAsync(withdrawnFromAccount, token));
                            break;

                        case AddedInterestsToAccount addedInterestsToAccount:
                            await UpdateViewAsync(view => view.HandleAsync(addedInterestsToAccount, token));
                            break;

                        case AccountFrozen accountFrozen:
                            await UpdateViewAsync(view => view.HandleAsync(accountFrozen, token));
                            break;

                        case AccountUnfrozen accountUnfrozen:
                            await UpdateViewAsync(view => view.HandleAsync(accountUnfrozen, token));
                            break;
                    }

                    _healthCheck.SetLastProcessTime();
                },
                subscriptionDropped: (subscription, reason, exception) =>
                {
                    SubscribeAsync(token).Wait(token);
                },
                cancellationToken: token);
        }

        private static bool IsSystemEvent(ResolvedEvent resolvedEvent)
        {
            return resolvedEvent.Event.EventType.StartsWith("$");
        }

        private async Task UpdateViewAsync(Func<AccountView, Task> handleAsync)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var accountView = scope.ServiceProvider.GetRequiredService<AccountView>();
            await handleAsync(accountView);
        }

        public override Task StopAsync(CancellationToken token)
        {
            _subscription?.Dispose();
            return base.StopAsync(token);
        }
    }
}