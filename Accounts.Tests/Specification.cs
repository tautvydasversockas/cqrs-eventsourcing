using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounts.Domain;
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
        protected virtual IEnumerable<Event> Given() => Enumerable.Empty<Event>();
        protected abstract TCommand When();
        protected virtual IEnumerable<Event> Then() => Enumerable.Empty<Event>();
        protected virtual bool Then_Fail() => false;

        [Test]
        public async Task Run()
        {
            var eventStoreMock = new Mock<IEventStore>();

            eventStoreMock
                .Setup(eventStore => eventStore.GetAggregateEventsAsync<TEventSourcedAggregate>(
                    It.IsAny<Guid>()))
                .Returns(Given().ToAsyncEnumerable);

            eventStoreMock
                .Setup(eventStore => eventStore.SaveAggregateEventsAsync<TEventSourcedAggregate>(
                    It.IsAny<Guid>(),
                    It.IsAny<IReadOnlyCollection<Event>>()))
                .Callback<Guid, IReadOnlyCollection<Event>>((_, events) => events.Should().BeEquivalentTo(Then()));

            var serviceProvider = Testing.GetServiceProvider(services =>
                services.Replace(new(typeof(IEventStore), _ => eventStoreMock.Object, ServiceLifetime.Scoped)));

            var mediator = serviceProvider.GetRequiredService<Mediator>();

            try
            {
                await mediator.SendAsync(When());
            }
            catch (InvalidOperationException)
            {
                Then_Fail().Should().BeTrue();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"SPECIFICATION: {GetType().Name.Replace('_', ' ')}");
            sb.AppendLine();

            if (Given().Any())
            {
                sb.AppendLine("GIVEN:");
                sb.AppendLine();

                foreach (var (@event, i) in Given().Select((@event, i) => (@event, i)))
                    sb.AppendLine($"{i + 1}. {@event}");

                sb.AppendLine();
            }

            sb.AppendLine("WHEN:");
            sb.AppendLine();
            sb.AppendLine($"{When()}");
            sb.AppendLine();

            sb.AppendLine("THEN:");
            sb.AppendLine();

            if (Then_Fail())
            {
                sb.AppendLine("Failed");
            }
            else
            {
                if (Then().Any())
                {
                    foreach (var (@event, i) in Then().Select((@event, i) => (@event, i)))
                        sb.AppendLine($"{i + 1}. {@event}");
                }
                else
                {
                    sb.AppendLine("Nothing happened");
                }
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}