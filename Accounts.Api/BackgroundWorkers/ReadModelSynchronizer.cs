using System;
using System.Linq;
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
        private EventStorePersistentSubscriptionBase? _subscription;

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

        private Task EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvt)
        {
            if (IsSystemEvent(resolvedEvt))
                return Task.CompletedTask;

            var (evt, _) = _serializer.Deserialize(resolvedEvt);

            return evt switch
            {
                AccountOpened e => _readModelGenerator.Handle(e),
                DepositedToAccount e => _readModelGenerator.Handle(e),
                WithdrawnFromAccount e => _readModelGenerator.Handle(e),
                AddedInterestsToAccount e => _readModelGenerator.Handle(e),
                AccountFrozen e => _readModelGenerator.Handle(e),
                AccountUnFrozen e => _readModelGenerator.Handle(e),
                _ => Task.CompletedTask
            };
        }

        private static bool IsSystemEvent(ResolvedEvent resolvedEvt)
        {
            var recordedEvt = resolvedEvt.Event;
            return !recordedEvt.Data.Any() || !recordedEvt.IsJson || recordedEvt.EventType.StartsWith('$');
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