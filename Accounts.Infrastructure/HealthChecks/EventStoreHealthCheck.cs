using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System;

namespace Accounts.Infrastructure.HealthChecks
{
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
                return new HealthCheckResult(context.Registration.FailureStatus, exception: e);
            }

            return HealthCheckResult.Healthy();
        }
    }
}