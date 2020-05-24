using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Accounts.Api.HealthChecks
{
    public sealed class HealthCheckResponseWriter
    {
        private const string DefaultResponse = "{}";

        public static Task WriteAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var response = report == null
                ? DefaultResponse
                : JsonSerializer.Serialize(new
                {
                    Status = report.Status.ToString(),
                    Entries = report.Entries.Select(entry => new
                    {
                        Key = entry.Key,
                        Status = entry.Value.Status.ToString()
                    })
                });
            return context.Response.WriteAsync(response);
        }
    }
}