using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Accounts.Infrastructure.HealthChecks
{
    public static class HealthCheckExtensions
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks(
                path: "/health",
                options: new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = (context, report) =>
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
                });

            app.UseHealthChecks(
                path: "/health/live",
                options: new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(HealthCheckTag.Liveness)
                });

            app.UseHealthChecks(
                path: "/health/ready",
                options: new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains(HealthCheckTag.Readiness)
                });

            return app;
        }
    }
}