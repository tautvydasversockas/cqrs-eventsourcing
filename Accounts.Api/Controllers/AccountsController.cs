using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Api.Dto;
using Accounts.Domain.Commands;
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
        private readonly CommandBus _commandBus;
        private readonly IAccountReadModel _readModel;
        private readonly MessageContext _context;

        public AccountsController(CommandBus commandBus, IAccountReadModel readModel, MessageContext context)
        {
            _commandBus = commandBus;
            _readModel = readModel;
            _context = context;
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
            return accountDto == null ? (IActionResult)NotFound() : Ok(accountDto);
        }

        [HttpPost("open")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Open(OpenAccountDto requestDto, CancellationToken token)
        {
            var id = DeterministicGuid.Create(_context.Id);
            await _commandBus.SendAsync(new OpenAccount(id, requestDto.ClientId, requestDto.InterestRate, requestDto.Balance), token);
            return CreatedAtRoute(nameof(Get), new { id }, id);
        }

        [HttpPost("{id}/deposit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Deposit(Guid id, DepositToAccountDto requestDto, CancellationToken token)
        {
            await _commandBus.SendAsync(new DepositToAccount(id, requestDto.Amount), token);
            return Ok();
        }

        [HttpPost("{id}/withdraw")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Withdraw(Guid id, WithdrawFromAccountDto requestDto, CancellationToken token)
        {
            await _commandBus.SendAsync(new WithdrawFromAccount(id, requestDto.Amount), token);
            return Ok();
        }

        [HttpPost("{id}/add-interests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddInterests(Guid id, CancellationToken token)
        {
            await _commandBus.SendAsync(new AddInterestsToAccount(id), token);
            return Ok();
        }

        [HttpPost("{id}/freeze")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Freeze(Guid id, CancellationToken token)
        {
            await _commandBus.SendAsync(new FreezeAccount(id), token);
            return Ok();
        }

        [HttpPost("{id}/unfreeze")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Unfreeze(Guid id, CancellationToken token)
        {
            await _commandBus.SendAsync(new UnfreezeAccount(id), token);
            return Ok();
        }
    }
}