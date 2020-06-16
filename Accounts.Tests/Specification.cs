using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounts.Domain.Common;
using Accounts.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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

            var repositoryMock = new Mock<IEventSourcedRepository<TEventSourcedAggregate>>();

            TEventSourcedAggregate? storedAggregate = null;

            if (Given().Any())
            {
                storedAggregate = new TEventSourcedAggregate();
                storedAggregate.LoadFromHistory(Given());
            }

            repositoryMock
                .Setup(repository => repository.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(storedAggregate);

            var updatedAggregate = new TEventSourcedAggregate();

            repositoryMock
                .Setup(repository => repository.SaveAsync(It.IsAny<TEventSourcedAggregate>()))
                .Callback<TEventSourcedAggregate>(aggregate => updatedAggregate = aggregate);

            var services = Testing.GetServices();

            var repositoryDescriptor = services.FirstOrDefault(descriptor =>
                descriptor.ServiceType == typeof(IEventSourcedRepository<>));
            services.Remove(repositoryDescriptor);
            services.AddScoped(_ => repositoryMock.Object);

            var serviceProvider = services.BuildServiceProvider();
            var commandBus = serviceProvider.GetService<CommandBus>();

            try
            {
                await commandBus.SendAsync(When());
            }
            catch (InvalidOperationException)
            {
                Then_Fail().Should().NotBeNull();
                return;
            }

            var publishedEvents = updatedAggregate.GetUncommittedEvents().ToList();
            publishedEvents.ForEach(@event => @event.Version = 0);

            publishedEvents.Should().HaveSameCount(Then());

            foreach (var @event in Then())
                publishedEvents.Should().ContainEquivalentOf(@event);
        }

        public override string ToString()
        {
            Before();

            var sb = new StringBuilder();

            sb.AppendLine($"SPECIFICATION: {GetType().Name.Replace('_', ' ')}\n");

            if (Given().Any())
            {
                sb.AppendLine("GIVEN:\n");

                foreach (var (@event, i) in Given().Select((@event, i) => (@event, i)))
                    sb.AppendLine($"{i + 1}. {@event}");
            }

            sb.AppendLine("WHEN:\n");
            sb.AppendLine($"{When()}");

            sb.AppendLine("THEN:\n");

            if (Then_Fail() == null)
            {
                if (Then().Any())
                {
                    foreach (var (@event, i) in Then().Select((@event, i) => (@event, i)))
                        sb.AppendLine($"{i + 1}. {@event}");
                }
                else
                {
                    sb.AppendLine("Nothing happened\n");
                }
            }
            else
            {
                sb.AppendLine($"{Then_Fail()}\n");
            }

            return sb.ToString();
        }
    }
}