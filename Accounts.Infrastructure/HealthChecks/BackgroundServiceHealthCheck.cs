﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                : new HealthCheckResult(context.Registration.FailureStatus));
        }
    }
}