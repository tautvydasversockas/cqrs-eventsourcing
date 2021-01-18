using System.Web.Http;
using Accounts.Application.Common.Exceptions;
using Accounts.Domain;
using Accounts.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Accounts.Api.MvcFilters
{
    public sealed class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = context.Exception switch
            {
                EntityNotFoundException e => new NotFoundObjectResult(e.Message),
                DuplicateKeyException e => new ConflictObjectResult(e.Message),
                DuplicateRequestException e => new ConflictObjectResult(e.Message),
                DomainException e => new ConflictObjectResult(e.Message),
                _ => new InternalServerErrorResult()
            };
        }
    }
}