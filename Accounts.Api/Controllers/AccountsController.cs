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

        var command = new OpenAccount(
            new ClientId(request.ClientId), 
            new InterestRate(request.InterestRate));

        var id = await _sender.Send(command, token);
        return CreatedAtRoute(nameof(Get), new { id.Value }, id.Value);
    }

    [HttpPost("{id}/deposit")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Deposit(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, DepositToAccountDto request, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new DepositToAccount(
            new AccountId(id), 
            new Money(request.Amount));

        await _sender.Send(command, token);
        return Ok();
    }

    [HttpPost("{id}/withdraw")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Withdraw(
        [FromHeader(Name = Headers.RequestId)] Guid requestId, Guid id, WithdrawFromAccountDto request, CancellationToken token)
    {
        RequestContext.RequestId = requestId;
        RequestContext.CausationId = requestId;
        RequestContext.CorrelationId = requestId;

        var command = new WithdrawFromAccount(
            new AccountId(id),
            new Money(request.Amount));

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

        var command = new AddInterestsToAccount(
            new AccountId(id));

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

        var command = new FreezeAccount(
            new AccountId(id));

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

        var command = new UnfreezeAccount(
            new AccountId(id));

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

        var command = new CloseAccount(
            new AccountId(id));

        await _sender.Send(command, token);
        return Ok();
    }
}
