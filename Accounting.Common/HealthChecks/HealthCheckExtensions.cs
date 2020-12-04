using Accounting.Common.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Accounting.Common.HealthChecks
{
    public static class HealthCheckExtensions
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = HealthCheckResponseWriter.WriteAsync
            });

            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckTag.Liveness)
            });

            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckTag.Readiness)
            });

            return app;
        }
    }
}