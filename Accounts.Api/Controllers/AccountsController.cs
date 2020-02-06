using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Commands;
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
        [ProducesResponseType(typeof(List<ActiveAccount>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var accounts = await _readModel.Accounts.ToListAsync(token);
            return Ok(accounts);
        }

        [HttpGet("{id}", Name = nameof(Get))]
        [ProducesResponseType(typeof(ActiveAccount), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([Required]Guid id, CancellationToken token)
        {
            var account = await _readModel.Accounts.SingleOrDefaultAsync(o => o.Id == id, token);
            return account == null ? (IActionResult)NotFound() : Ok(account);
        }

        [HttpPost("open")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Open([Required]OpenAccount cmd, CancellationToken token)
        {
            return Ok(await _mediator.Send(cmd, token));
        }

        [HttpPost("deposit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Deposit([Required]DepositToAccount cmd, CancellationToken token)
        {
            return Ok(await _mediator.Send(cmd, token));
        }

        [HttpPost("withdraw")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Withdraw([Required]WithdrawFromAccount cmd, CancellationToken token)
        {
            return Ok(await _mediator.Send(cmd, token));
        }

        [HttpPost("add-interests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddInterests([Required]AddInterestsToAccount cmd, CancellationToken token)
        {
            return Ok(await _mediator.Send(cmd, token));
        }

        [HttpPost("freeze")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Freeze([Required]FreezeAccount cmd, CancellationToken token)
        {
            return Ok(await _mediator.Send(cmd, token));
        }

        [HttpPost("unfreeze")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Unfreeze([Required]UnFreezeAccount cmd, CancellationToken token)
        {
            return Ok(await _mediator.Send(cmd, token));
        }
    }
}