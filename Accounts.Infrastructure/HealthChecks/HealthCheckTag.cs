﻿namespace Accounts.Infrastructure.HealthChecks
{
    public static class HealthCheckTag
    {
        public const string Readiness = "ready";
        public const string Liveness = "live";
    }
}