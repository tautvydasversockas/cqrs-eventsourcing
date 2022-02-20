namespace Accounts.Api.Controllers;

[ApiController]
[Route("api/v1/accounts")]
public sealed class AccountsController : Controller
{
    private readonly ISender _sender;
    private readonly AccountReadModel _readModel;

    public AccountsController(ISender sender, AccountReadModel readModel)
    {
        _sender = sender;
        _readModel = readModel;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _readModel.GetAccountsAsync();
        return Ok(accounts);
    }

    [HttpGet("{id}", Name = nameof(Get))]
    [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get(Guid id)
    {
        var account = await _readModel.GetAccountByIdAsync(id);
        return account is null
            ? NotFound($"Entity 'Account' ({id}) was not found.")
            : Ok(account);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Open(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, OpenAccountDto request, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new OpenAccount(request.ClientId, request.InterestRate, request.Balance);
        var id = await _sender.Send(command, token);
        return CreatedAtRoute(nameof(Get), new { id }, id);
    }

    [HttpPost("{id}/deposit")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Deposit(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, DepositToAccountDto request, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new DepositToAccount(id, request.Amount);
        await _sender.Send(command, token);
        return Ok();
    }

    [HttpPost("{id}/withdraw")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Withdraw(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, WithdrawFromAccountDto requestDto, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new WithdrawFromAccount(id, requestDto.Amount);
        await _sender.Send(command, token);
        return Ok();
    }

    [HttpPost("{id}/add-interests")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddInterests(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new AddInterestsToAccount(id);
        await _sender.Send(command, token);
        return Ok();
    }

    [HttpPost("{id}/freeze")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Freeze(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new FreezeAccount(id);
        await _sender.Send(command, token);
        return Ok();
    }

    [HttpPost("{id}/unfreeze")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Unfreeze(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new UnfreezeAccount(id);
        await _sender.Send(command, token);
        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Close(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new CloseAccount(id);
        await _sender.Send(command, token);
        return Ok();
    }
}
