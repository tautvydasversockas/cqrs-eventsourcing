using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Accounts.Api.HealthChecks
{
    public sealed class HealthCheckResponseWriter
    {
        public static Task WriteAsync(HttpContext ctx, HealthReport report)
        {
            ctx.Response.ContentType = "application/json";
            var result = report == null ? "{}" : JsonSerializer.Serialize(new
            {
                Status = report.Status.ToString(),
                Entries = report.Entries.Select(e => new
                {
                    Key = e.Key,
                    Status = e.Value.Status.ToString()
                })
            });
            return ctx.Response.WriteAsync(result);
        }
    }
}