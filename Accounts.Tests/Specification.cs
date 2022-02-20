namespace Accounts.Tests;

[TestFixture]
public abstract class Specification<TEventSourcedAggregate, TId, TCommand>
    where TEventSourcedAggregate : EventSourcedAggregate<TId>
    where TCommand : ICommand
{
    protected TEventSourcedAggregate? Aggregate;
    protected virtual void ConfigureServices(ServiceCollection services) { }
    protected virtual IEnumerable<IEvent> Given() => Enumerable.Empty<IEvent>();
    protected abstract TCommand When();
    protected virtual IEnumerable<IEvent> Then() => Enumerable.Empty<IEvent>();

    [Test]
    public async Task Run()
    {
        foreach (var @event in Given())
        {
            Aggregate ??= (TEventSourcedAggregate)Activator.CreateInstance(typeof(TEventSourcedAggregate), true)!;
            Aggregate.ApplyEvent(@event);
        }

        var services = Testing.GetServices();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        var sender = serviceProvider.GetRequiredService<ISender>();

        try
        {
            await sender.Send(When());
        }
        catch (Exception e)
        {
            IsErrorExpected(e).Should().BeTrue();
            return;
        }

        IsErrorExpected().Should().BeFalse();
        Aggregate.Should().NotBeNull();
        Aggregate!.UncommittedEvents.Should().BeEquivalentTo(Then(), options => options.RespectingRuntimeTypes());
    }

    private bool IsErrorExpected(Exception? e = null)
    {
        try
        {
            foreach (var _ in Then())
            {
                // Iterate to catch expected errors.
            }

            return false;
        }
        catch (Exception expectedError)
        {
            return e is null || e.GetType() == expectedError.GetType();
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        var specificationName = GetType().Name.Replace('_', ' ');

        sb.AppendLine($"SPECIFICATION: {specificationName}");
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

        if (IsErrorExpected())
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
