namespace Accounts.Infrastructure.HealthChecks;

public static class HealthCheckExtensions
{
    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                Predicate = _ => true
            });

        app.UseHealthChecks(
            "/health/live",
            new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckTag.Liveness)
            });

        app.UseHealthChecks(
            "/health/ready",
            new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckTag.Readiness)
            });

        return app;
    }
}
