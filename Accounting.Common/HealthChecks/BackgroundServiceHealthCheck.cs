using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Accounting.Common.HealthChecks
{
    public sealed class BackgroundServiceHealthCheck : IHealthCheck
    {
        private readonly TimeSpan _timeout;
        private long _lastProcessTime;

        public BackgroundServiceHealthCheck(TimeSpan timeout)
        {
            _timeout = timeout;
            SetLastProcessTime();
        }

        public void SetLastProcessTime()
        {
            Interlocked.Exchange(ref _lastProcessTime, DateTime.Now.Ticks);
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
        {
            return Task.FromResult(Interlocked.Read(ref _lastProcessTime) > DateTime.Now.Ticks - _timeout.Ticks
                ? HealthCheckResult.Healthy("Processing.")
                : new HealthCheckResult(context.Registration.FailureStatus, "Not processing."));
        }
    }
}