using Accounts.Api.Middleware;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Accounts.Api.OpenApiFilters
{
    public sealed class AddRequiredHeaderParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = RequestHeaderNames.RequestId,
                In = ParameterLocation.Header
            });
        }
    }
}