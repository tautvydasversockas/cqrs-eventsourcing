using System.Web.Http;
using Infrastructure.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Accounts.Api.Filters
{
    public sealed class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext ctx)
        {
            switch (ctx.Exception)
            {
                case EntityNotFoundException _:
                    ctx.Result = new NotFoundResult();
                    break;

                default:
                    ctx.Result = new InternalServerErrorResult();
                    break;
            }
        }
    }
}