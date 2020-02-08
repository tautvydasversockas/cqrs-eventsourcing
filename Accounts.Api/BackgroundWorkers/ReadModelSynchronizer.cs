using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain.Events;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Accounts.Api.BackgroundWorkers
{
    public sealed class ReadModelSynchronizer : BackgroundService
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreSerializer _serializer;
        private readonly AccountReadModelGenerator _readModelGenerator;
        private EventStorePersistentSubscriptionBase _subscription;

        public ReadModelSynchronizer(
            IEventStoreConnection connection,
            EventStoreSerializer serializer,
            AccountReadModelGenerator readModelGenerator)
        {
            _connection = connection;
            _serializer = serializer;
            _readModelGenerator = readModelGenerator;
        }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            return SubscribeAsync();
        }

        private async Task SubscribeAsync()
        {
            _subscription = await _connection.ConnectToPersistentSubscriptionAsync("$ce-Account", "Account-ReadModel", EventAppeared, SubscriptionDropped);
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
        {
            if (IsSystemEvent(resolvedEvent))
                return Task.CompletedTask;

            var (evt, _) = _serializer.Deserialize(resolvedEvent);

            return evt switch
            {
                AccountOpened accountOpened => _readModelGenerator.Handle(accountOpened),
                DepositedToAccount depositedToAccount => _readModelGenerator.Handle(depositedToAccount),
                WithdrawnFromAccount withdrawnFromAccount => _readModelGenerator.Handle(withdrawnFromAccount),
                AddedInterestsToAccount addedInterestsToAccount => _readModelGenerator.Handle(addedInterestsToAccount),
                AccountFrozen accountFrozen => _readModelGenerator.Handle(accountFrozen),
                AccountUnFrozen depositedToAccount => _readModelGenerator.Handle(depositedToAccount),
                _ => Task.CompletedTask
            };
        }

        private static bool IsSystemEvent(ResolvedEvent resolvedEvent)
        {
            var recordedEvt = resolvedEvent.Event;
            return recordedEvt.Data.Length <= 0 || !recordedEvt.IsJson || recordedEvt.EventType.StartsWith("$");
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase subscription, SubscriptionDropReason reason, Exception e)
        {
            SubscribeAsync().GetAwaiter().GetResult();
        }

        public override Task StopAsync(CancellationToken token)
        {
            _subscription.Stop(TimeSpan.FromSeconds(5));
            return base.StopAsync(token);
        }
    }
}