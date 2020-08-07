using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Accounts.Api.HealthChecks
{
    public sealed class BackgroundServiceHealthCheck : IHealthCheck
    {
        private static long CurrentTime => DateTime.Now.Ticks;
        private readonly long _timeBetweenProcess;
        private long _lastProcessTime;

        public BackgroundServiceHealthCheck(TimeSpan timeBetweenProcess)
        {
            _timeBetweenProcess = timeBetweenProcess.Ticks;
            SetLastProcessTime();
        }

        public void SetLastProcessTime()
        {
            Interlocked.Exchange(ref _lastProcessTime, CurrentTime);
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
        {
            return Task.FromResult(Interlocked.Read(ref _lastProcessTime) > CurrentTime - _timeBetweenProcess
                ? HealthCheckResult.Healthy("Processing.")
                : new HealthCheckResult(context.Registration.FailureStatus, "Not processing."));
        }
    }
}