using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.HealthChecks
{
    public static class HealthCheckExtensions
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks(
                "/health",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = WriteHealthCheckResponseAsync
                });

            app.UseHealthChecks(
                "/health/live",
                new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(HealthCheckTag.Liveness),
                    ResponseWriter = WriteHealthCheckResponseAsync
                });

            app.UseHealthChecks(
                "/health/ready",
                new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(HealthCheckTag.Readiness),
                    ResponseWriter = WriteHealthCheckResponseAsync
                });

            return app;
        }

        private static Task WriteHealthCheckResponseAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = report is null
                ? "{}"
                : JsonSerializer.Serialize(new
                {
                    Status = report.Status.ToString(),
                    Entries = report.Entries.Select(entry => new
                    {
                        entry.Key,
                        Status = entry.Value.Status.ToString()
                    })
                });

            return context.Response.WriteAsync(response);
        }
    }
}