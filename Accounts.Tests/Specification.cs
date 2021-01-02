using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain;
using Accounts.Domain.Common;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;

namespace Accounts.Tests
{
    [TestFixture]
    public abstract class Specification<TEventSourcedAggregate, TId, TCommand>
        where TEventSourcedAggregate : EventSourcedAggregate<TId>, new()
        where TCommand : IRequest
        where TId : notnull
    {
        protected virtual IEnumerable<IEvent> Given() => Enumerable.Empty<IEvent>();
        protected abstract TCommand When();
        protected virtual IEnumerable<IEvent> Then() => Enumerable.Empty<IEvent>();
        protected virtual bool Then_Fail() => false;

        [Test]
        public async Task Run()
        {
            var eventSourcedRepositoryMock = new Mock<IEventSourcedRepository<TEventSourcedAggregate, TId>>();

            eventSourcedRepositoryMock
                .Setup(eventSourcedRepository => eventSourcedRepository.GetAsync(
                    It.IsAny<TId>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    TEventSourcedAggregate? aggregate = null;

                    foreach (var @event in Given())
                        (aggregate ??= new()).ApplyEvent(@event);

                    return aggregate;
                });

            eventSourcedRepositoryMock
                .Setup(eventSourcedRepository => eventSourcedRepository.SaveAsync(
                    It.IsAny<TEventSourcedAggregate>(),
                    It.IsAny<CancellationToken>()))
                .Callback<TEventSourcedAggregate, CancellationToken>((aggregate, _) =>
                    aggregate.UncommittedEvents.Should().BeEquivalentTo(
                        Then(), options => options.RespectingRuntimeTypes()));

            var serviceProvider = Testing.GetServiceProvider(services =>
                services.Replace(new(
                    typeof(IEventSourcedRepository<TEventSourcedAggregate, TId>),
                    _ => eventSourcedRepositoryMock.Object,
                    ServiceLifetime.Scoped)));

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            try
            {
                await mediator.Send(When());
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