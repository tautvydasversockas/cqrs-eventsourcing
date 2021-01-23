using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Api.Dto;
using Accounts.Application.Common.Exceptions;
using Accounts.Domain;
using Accounts.Infrastructure;
using Accounts.ReadModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
    public sealed class AccountsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAccountReadModel _readModel;

        public AccountsController(IMediator mediator, IAccountReadModel readModel)
        {
            _mediator = mediator;
            _readModel = readModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var accounts = await _readModel.Accounts.ToListAsync(token);
            return Ok(accounts);
        }

        [HttpGet("{id}", Name = nameof(Get))]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(Guid id, CancellationToken token)
        {
            var account = await _readModel.Accounts.SingleOrDefaultAsync(account => account.Id == id, token) ??
                throw new EntityNotFoundException(nameof(Account), id);
            return Ok(account);
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
            var id = await _mediator.Send(command, token);
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
            await _mediator.Send(command, token);
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
            await _mediator.Send(command, token);
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
            await _mediator.Send(command, token);
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
            await _mediator.Send(command, token);
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
            await _mediator.Send(command, token);
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
            await _mediator.Send(command, token);
            return Ok();
        }
    }
}