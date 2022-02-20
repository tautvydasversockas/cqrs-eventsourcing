namespace Accounts.Api.MvcFilters;

public sealed class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            EntityNotFoundException e => new NotFoundObjectResult(e.Message),
            Infrastructure.DuplicateKeyException e => new ConflictObjectResult(e.Message),
            DuplicateRequestException e => new ConflictObjectResult(e.Message),
            DomainException e => new ConflictObjectResult(e.Message),
            _ => new InternalServerErrorResult()
        };
    }
}
