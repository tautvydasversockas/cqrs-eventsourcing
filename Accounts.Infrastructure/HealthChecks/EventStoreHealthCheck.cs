namespace Accounts.Infrastructure.HealthChecks;

public sealed class EventStoreHealthCheck : IHealthCheck
{
    private readonly EventStoreClient _client;

    public EventStoreHealthCheck(EventStoreClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
    {
        try
        {
            await _client
                .ReadAllAsync(Direction.Forwards, Position.Start, maxCount: 1, cancellationToken: token)
                .FirstOrDefaultAsync(token);
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(exception: e);
        }

        return HealthCheckResult.Healthy();
    }
}
