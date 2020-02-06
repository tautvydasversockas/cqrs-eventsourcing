using System;
using System.Collections.Generic;
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
        private readonly AccountReadModelGenerator _readModelGenerator;
        private EventStorePersistentSubscriptionBase _subscription;

        public ReadModelSynchronizer(
            IEventStoreConnection connection, 
            AccountReadModelGenerator readModelGenerator)
        {
            _connection = connection;
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
            var recordedEvt = resolvedEvent.Event;
            var metadata = EventStoreSerializer.Deserialize<Dictionary<string, object>>(recordedEvt.Metadata);
            var evtType = Type.GetType(metadata[EventStoreMetadataKeys.EventClrType].ToString());
            var evt = EventStoreSerializer.Deserialize(recordedEvt.Data, evtType);
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