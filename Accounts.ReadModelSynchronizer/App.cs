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

        private PersistentSubscription? _subscription;

        public App(EventStorePersistentSubscriptionsClient client, IServiceScopeFactory serviceScopeFactory)
        {
            _client = client;
            _serviceScopeFactory = serviceScopeFactory;
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
                    var eventRecord = resolvedEvent.Event;
                    if (IsSystemEvent(eventRecord))
                        return;

                    BackgroundServiceStatistics.SetLastProcessTime();

                    var (@event, _) = EventStoreSerializer.Deserialize(eventRecord);

                    switch (@event)
                    {
                        case AccountOpened accountOpened:
                            {
                                await UpdateViewAsync(view => view.HandleAsync(accountOpened, token));
                                break;
                            }
                        case DepositedToAccount depositedToAccount:
                            {
                                var version = (int)eventRecord.EventNumber.ToUInt64();
                                await UpdateViewAsync(view => view.HandleAsync(depositedToAccount, version, token));
                                break;
                            }
                        case WithdrawnFromAccount withdrawnFromAccount:
                            {
                                var version = (int)eventRecord.EventNumber.ToUInt64();
                                await UpdateViewAsync(view => view.HandleAsync(withdrawnFromAccount, version, token));
                                break;
                            }
                        case AddedInterestsToAccount addedInterestsToAccount:
                            {
                                var version = (int)eventRecord.EventNumber.ToUInt64();
                                await UpdateViewAsync(view => view.HandleAsync(addedInterestsToAccount, version, token));
                                break;
                            }
                        case AccountFrozen accountFrozen:
                            {
                                var version = (int)eventRecord.EventNumber.ToUInt64();
                                await UpdateViewAsync(view => view.HandleAsync(accountFrozen, version, token));
                                break;
                            }
                        case AccountUnfrozen accountUnfrozen:
                            {
                                var version = (int)eventRecord.EventNumber.ToUInt64();
                                await UpdateViewAsync(view => view.HandleAsync(accountUnfrozen, version, token));
                                break;
                            }
                        case AccountClosed accountClosed:
                            {
                                await UpdateViewAsync(view => view.HandleAsync(accountClosed, token));
                                break;
                            }
                    }
                },
                subscriptionDropped: (subscription, reason, exception) =>
                {
                    SubscribeAsync(token).Wait(token);
                },
                cancellationToken: token);
        }

        private static bool IsSystemEvent(EventRecord eventRecord)
        {
            return eventRecord.EventType.StartsWith("$");
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