using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Accounts.Infrastructure.HealthChecks
{
    public sealed class BackgroundServiceHealthCheck : IHealthCheck
    {
        private readonly TimeSpan _timeout;
        public DateTime LastProcessTime { get; private set; } = DateTime.Now;
        public void SetLastProcessTime() => LastProcessTime = DateTime.Now;

        public BackgroundServiceHealthCheck(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
        {
            return Task.FromResult<HealthCheckResult>(LastProcessTime.Add(_timeout) > DateTime.Now
                ? new(HealthStatus.Healthy)
                : new(context.Registration.FailureStatus));
        }
    }
}