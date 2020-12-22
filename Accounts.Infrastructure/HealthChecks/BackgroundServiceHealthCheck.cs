using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Accounts.Infrastructure.HealthChecks
{
    public sealed class BackgroundServiceHealthCheck : IHealthCheck
    {
        private readonly TimeSpan _timeout;

        public BackgroundServiceHealthCheck(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
        {
            return Task.FromResult(BackgroundServiceStatistics.LastProcessTime.Add(_timeout) > DateTime.Now
                ? HealthCheckResult.Healthy()
                : new(context.Registration.FailureStatus));
        }
    }
}