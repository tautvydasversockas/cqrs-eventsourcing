namespace Accounts.ReadModelSynchronizer;

public sealed class App : BackgroundService
{
    private readonly EventStorePersistentSubscriptionsClient _client;
    private readonly AccountView _accountView;

    private PersistentSubscription? _subscription;

    public App(EventStorePersistentSubscriptionsClient client, AccountView accountView)
    {
        _client = client;
        _accountView = accountView;
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

                var @event = EventStoreSerializer.DeserializeEvent(eventRecord);
                var version = (int)eventRecord.EventNumber.ToUInt64();

                switch (@event)
                {
                    case AccountOpened accountOpened:
                        await _accountView.HandleAsync(accountOpened);
                        break;

                    case DepositedToAccount depositedToAccount:
                        await _accountView.HandleAsync(depositedToAccount, version);
                        break;

                    case WithdrawnFromAccount withdrawnFromAccount:
                        await _accountView.HandleAsync(withdrawnFromAccount, version);
                        break;

                    case AddedInterestsToAccount addedInterestsToAccount:
                        await _accountView.HandleAsync(addedInterestsToAccount, version);
                        break;

                    case AccountFrozen accountFrozen:
                        await _accountView.HandleAsync(accountFrozen, version);
                        break;

                    case AccountUnfrozen accountUnfrozen:
                        await _accountView.HandleAsync(accountUnfrozen, version);
                        break;

                    case AccountClosed accountClosed:
                        await _accountView.HandleAsync(accountClosed);
                        break;
                }

                BackgroundServiceStatistics.SetLastProcessTime();
            },
            subscriptionDropped: (subscription, reason, exception) =>
            {
                SubscribeAsync(token).Wait(token);
            },
            cancellationToken: token);
    }

    private static bool IsSystemEvent(EventRecord eventRecord)
    {
        return eventRecord.EventType[0] == '$';
    }

    public override Task StopAsync(CancellationToken token)
    {
        _subscription?.Dispose();
        return base.StopAsync(token);
    }
}
