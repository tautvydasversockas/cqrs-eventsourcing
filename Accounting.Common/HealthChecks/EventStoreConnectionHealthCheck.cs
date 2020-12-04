using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Accounting.Common.HealthChecks
{
    public sealed class EventStoreConnectionHealthCheck : IHealthCheck
    {
        private volatile bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set => _isConnected = value;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
        {
            return Task.FromResult(IsConnected
                ? HealthCheckResult.Healthy("Connected.")
                : new HealthCheckResult(context.Registration.FailureStatus, "Not connected."));
        }
    }
}