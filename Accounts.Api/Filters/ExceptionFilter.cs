using System.Web.Http;
using Accounts.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Accounts.Api.Filters
{
    public sealed class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case EntityNotFoundException _:
                    context.Result = new NotFoundResult();
                    break;

                default:
                    context.Result = new InternalServerErrorResult();
                    break;
            }
        }
    }
}