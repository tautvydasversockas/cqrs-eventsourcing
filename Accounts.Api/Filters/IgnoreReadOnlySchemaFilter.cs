using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Accounts.Api.Filters
{
    public sealed class IgnoreReadOnlySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.ReadOnly = false;

            foreach (var value in schema.Properties.Values)
                value.ReadOnly = false;
        }
    }
}