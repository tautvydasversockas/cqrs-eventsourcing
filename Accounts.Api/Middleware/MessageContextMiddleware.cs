using System.Net;
using System.Threading.Tasks;
using Accounts.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Accounts.Api.Middleware
{
    public sealed class MessageContextMiddleware
    {
        private readonly RequestDelegate _next;

        public MessageContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, MessageContextProvider messageContextProvider)
        {
            if (context.Request.Headers.TryGetValue(RequestHeaderNames.RequestId, out var requestId))
            {
                messageContextProvider.Context = new MessageContext(requestId, requestId, requestId);
                await _next(context);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync($"{RequestHeaderNames.RequestId} header is missing.");
            }
        }
    }
}