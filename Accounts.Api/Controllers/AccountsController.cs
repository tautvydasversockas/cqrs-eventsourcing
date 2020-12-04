﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Api.Dto;
using Accounts.Domain;
using Accounts.Infrastructure;
using Accounts.ReadModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
    public sealed class AccountsController : Controller
    {
        public const string RequestId = "X-Request-ID";

        private readonly Mediator _mediator;
        private readonly IAccountReadModel _readModel;

        public AccountsController(Mediator mediator, IAccountReadModel readModel)
        {
            _mediator = mediator;
            _readModel = readModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var accountDtos = await _readModel.Accounts.ToListAsync(token);
            return Ok(accountDtos);
        }

        [HttpGet("{id}", Name = nameof(Get))]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(Guid id, CancellationToken token)
        {
            var accountDto = await _readModel.Accounts.SingleOrDefaultAsync(account => account.Id == id, token);
            return accountDto is null ? NotFound() : Ok(accountDto);
        }

        [HttpPost("open")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Open([FromHeader(Name = RequestId)] string requestId, OpenAccountDto requestDto, CancellationToken token)
        {
            MessageContext.MessageId = requestId;
            MessageContext.CausationId = requestId;
            MessageContext.CorrelationId = requestId;

            var id = DeterministicGuid.Create(requestId);
            var command = new OpenAccount(id, requestDto.ClientId, requestDto.InterestRate, requestDto.Balance);
            await _mediator.SendAsync(command, token);
            return CreatedAtRoute(nameof(Get), new { id }, id);
        }

        [HttpPost("{id}/deposit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Deposit([FromHeader(Name = RequestId)] string requestId, Guid id, DepositToAccountDto requestDto, CancellationToken token)
        {
            MessageContext.MessageId = requestId;
            MessageContext.CausationId = requestId;
            MessageContext.CorrelationId = requestId;

            var command = new DepositToAccount(id, requestDto.Amount);
            await _mediator.SendAsync(command, token);
            return Ok();
        }

        [HttpPost("{id}/withdraw")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Withdraw([FromHeader(Name = RequestId)] string requestId, Guid id, WithdrawFromAccountDto requestDto, CancellationToken token)
        {
            MessageContext.MessageId = requestId;
            MessageContext.CausationId = requestId;
            MessageContext.CorrelationId = requestId;

            var command = new WithdrawFromAccount(id, requestDto.Amount);
            await _mediator.SendAsync(command, token);
            return Ok();
        }

        [HttpPost("{id}/add-interests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddInterests([FromHeader(Name = RequestId)] string requestId, Guid id, CancellationToken token)
        {
            MessageContext.MessageId = requestId;
            MessageContext.CausationId = requestId;
            MessageContext.CorrelationId = requestId;

            var command = new AddInterestsToAccount(id);
            await _mediator.SendAsync(command, token);
            return Ok();
        }

        [HttpPost("{id}/freeze")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Freeze([FromHeader(Name = RequestId)] string requestId, Guid id, CancellationToken token)
        {
            MessageContext.MessageId = requestId;
            MessageContext.CausationId = requestId;
            MessageContext.CorrelationId = requestId;

            var command = new FreezeAccount(id);
            await _mediator.SendAsync(command, token);
            return Ok();
        }

        [HttpPost("{id}/unfreeze")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Unfreeze([FromHeader(Name = RequestId)] string requestId, Guid id, CancellationToken token)
        {
            MessageContext.MessageId = requestId;
            MessageContext.CausationId = requestId;
            MessageContext.CorrelationId = requestId;

            var command = new UnfreezeAccount(id);
            await _mediator.SendAsync(command, token);
            return Ok();
        }
    }
}