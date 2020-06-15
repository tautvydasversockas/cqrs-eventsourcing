using System;
using System.Web.Http;
using Accounts.Application.Common.Exceptions;
using Accounts.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Accounts.Api.MvcFilters
{
    public sealed class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case EntityNotFoundException e:
                    context.Result = new NotFoundObjectResult(e.Message);
                    break;

                case DuplicateKeyException e:
                    context.Result = new ConflictObjectResult(e.Message);
                    break;

                case DuplicateOperationException e:
                    context.Result = new ConflictObjectResult(e.Message);
                    break;

                case InvalidOperationException e:
                    context.Result = new BadRequestObjectResult(e.Message);
                    break;

                default:
                    context.Result = new InternalServerErrorResult();
                    break;
            }
        }
    }
}