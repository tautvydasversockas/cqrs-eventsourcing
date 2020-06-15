using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Domain.Common;
using Accounts.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Accounts.Tests
{
    public sealed class ThenAttribute : TestAttribute { }

    [TestFixture]
    public abstract class Specification<TEventSourcedAggregate, TCommand>
        where TEventSourcedAggregate : EventSourcedAggregate, new()
        where TCommand : Command
    {
        protected virtual IEnumerable<Event> Given() => Enumerable.Empty<Event>();
        protected abstract TCommand When();

        protected TCommand Command;
        protected List<Event> PublishedEvents = new List<Event>();
        protected Exception? CaughtException;

        [SetUp]
        protected async Task Setup()
        {
            var repositoryMock = new Mock<IEventSourcedRepository<TEventSourcedAggregate>>();

            TEventSourcedAggregate? storedAggregate = null;
            var history = Given().ToList();

            if (history.Any())
            {
                storedAggregate = new TEventSourcedAggregate();
                storedAggregate.LoadFromHistory(history);
            }

            repositoryMock
                .Setup(repository => repository.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(storedAggregate);

            var updatedAggregate = new TEventSourcedAggregate();

            repositoryMock
                .Setup(repository => repository.SaveAsync(It.IsAny<TEventSourcedAggregate>()))
                .Callback<TEventSourcedAggregate>(aggregate => updatedAggregate = aggregate);

            var serviceProvider = BuildServiceProvider(services =>
            {
                var repositoryDescriptor = services.FirstOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IEventSourcedRepository<>));
                services.Remove(repositoryDescriptor);
                services.AddScoped(_ => repositoryMock.Object);
            });

            var commandBus = serviceProvider.GetService<CommandBus>();

            try
            {
                Command = When();
                await commandBus.SendAsync(Command);
                PublishedEvents.AddRange(updatedAggregate.GetUncommittedEvents());
            }
            catch (Exception e)
            {
                CaughtException = e;
            }
        }

        private IServiceProvider BuildServiceProvider(Action<IServiceCollection> setup)
        {
            var services = Testing.GetServices();
            setup(services);
            return services.BuildServiceProvider();
        }

        protected void AssertPublished<TEvent>() where TEvent : Event
        {
            PublishedEvents.Should().Contain(@event => @event is TEvent);
        }

        protected TEvent GetFromPublished<TEvent>() where TEvent : Event
        {
            return PublishedEvents.First(@event => @event is TEvent).As<TEvent>();
        }

        protected void AssertFailed()
        {
            PublishedEvents.Should().BeEmpty();
            CaughtException.Should().NotBeNull();
        }
    }
}