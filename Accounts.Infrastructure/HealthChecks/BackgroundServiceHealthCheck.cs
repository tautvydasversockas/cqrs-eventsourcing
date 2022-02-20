namespace Accounts.Infrastructure.HealthChecks;

public sealed class BackgroundServiceHealthCheck : IHealthCheck
{
    private readonly TimeSpan _timeout;

    public BackgroundServiceHealthCheck(TimeSpan timeout)
    {
        _timeout = timeout;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
    {
        var result = IsTimedOut()
            ? HealthCheckResult.Unhealthy()
            : HealthCheckResult.Healthy();

        return Task.FromResult(result);
    }

    private bool IsTimedOut()
    {
        return BackgroundServiceStatistics.LastProcessTime.Add(_timeout) < DateTimeOffset.Now;
    }
}
