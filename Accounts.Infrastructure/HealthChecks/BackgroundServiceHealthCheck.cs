namespace Accounts.Infrastructure.HealthChecks;

public sealed class BackgroundServiceHealthCheck : IHealthCheck
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(1);
    public Func<DateTimeOffset, bool> WorkingHours { get; set; } = _ => true;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
    {
        var result = IsTimedOut()
            ? HealthCheckResult.Unhealthy()
            : HealthCheckResult.Healthy();

        return Task.FromResult(result);
    }

    private bool IsTimedOut()
    {
        var now = DateTimeOffset.UtcNow;
        if (!WorkingHours(now))
            return false;

        var lastProcessTime = BackgroundServiceStatistics.LastProcessTime;
        return lastProcessTime.Add(Timeout) < DateTimeOffset.UtcNow;
    }
}
