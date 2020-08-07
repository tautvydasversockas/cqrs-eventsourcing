using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounts.Domain.Common;
using Accounts.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;

namespace Accounts.Tests
{
    [TestFixture]
    public abstract class Specification<TEventSourcedAggregate, TCommand>
        where TEventSourcedAggregate : EventSourcedAggregate, new()
        where TCommand : Command
    {
        protected virtual void Before() { }
        protected virtual IEnumerable<Event> Given() => Enumerable.Empty<Event>();
        protected abstract TCommand When();
        protected virtual IEnumerable<Event> Then() => Enumerable.Empty<Event>();
        protected virtual string? Then_Fail() => null;

        [Test]
        public async Task Run()
        {
            Before();

            var eventStoreMock = new Mock<IEventStore>();

            eventStoreMock
                .Setup(eventStore => eventStore.GetAggregateEventsAsync<TEventSourcedAggregate>(
                    It.IsAny<Guid>()))
                .Returns(Given().ToAsyncEnumerable);

            eventStoreMock
                .Setup(eventStore => eventStore.SaveAggregateEventsAsync<TEventSourcedAggregate>(
                    It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Event>>(),
                    It.IsAny<int>()))
                .Callback<Guid, IEnumerable<Event>, int>((id, events, expectedVersion) => 
                    events.Should().BeEquivalentTo(Then(), options =>
                        options.RespectingRuntimeTypes().Excluding(@event => @event.Version)));

            var serviceProvider = Testing.GetServiceProvider(services =>
                services.Replace(new ServiceDescriptor(typeof(IEventStore), _ => eventStoreMock.Object, ServiceLifetime.Scoped)));

            var messageBus = serviceProvider.GetRequiredService<MessageBus>();
            var messageContext = new MessageContext(string.Empty, string.Empty, string.Empty);

            try
            {
                await messageBus.SendAsync(When(), messageContext);
            }
            catch (InvalidOperationException)
            {
                Then_Fail().Should().NotBeNull();
            }
        }

        public override string ToString()
        {
            Before();

            var sb = new StringBuilder();

            sb.AppendLine($"SPECIFICATION: {GetType().Name.Replace('_', ' ')}");
            sb.AppendLine();

            if (Given().Any())
            {
                sb.AppendLine("GIVEN:");
                sb.AppendLine();

                foreach (var (@event, i) in Given().Select((@event, i) => (@event, i)))
                    sb.AppendLine($"{i + 1}. {@event}");
            }

            sb.AppendLine("WHEN:");
            sb.AppendLine();
            sb.AppendLine($"{When()}");

            sb.AppendLine("THEN:");
            sb.AppendLine();

            if (Then_Fail() == null)
            {
                if (Then().Any())
                {
                    foreach (var (@event, i) in Then().Select((@event, i) => (@event, i)))
                        sb.AppendLine($"{i + 1}. {@event}");
                }
                else
                {
                    sb.AppendLine("Nothing happened");
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine($"{Then_Fail()}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}