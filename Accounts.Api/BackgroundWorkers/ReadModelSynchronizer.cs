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
            const string stream = "$ce-Account";
            const string groupName = "Account-ReadModel";
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
                    await _readModelGenerator.Handle(e);
                    break;

                case DepositedToAccount e:
                    await _readModelGenerator.Handle(e);
                    break;

                case WithdrawnFromAccount e:
                    await _readModelGenerator.Handle(e);
                    break;

                case AddedInterestsToAccount e:
                    await _readModelGenerator.Handle(e);
                    break;

                case AccountFrozen e:
                    await _readModelGenerator.Handle(e);
                    break;

                case AccountUnFrozen e:
                    await _readModelGenerator.Handle(e);
                    break;
            }
        }

        private static bool IsSystemEvent(ResolvedEvent resolvedEvent)
        {
            var recordedEvent = resolvedEvent.Event;
            return !recordedEvent.Data.Any() || !recordedEvent.IsJson || recordedEvent.EventType.StartsWith('$');
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