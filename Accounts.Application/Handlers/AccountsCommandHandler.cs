using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Commands;
using Accounts.Application.Common;
using Accounts.Domain;
using Infrastructure;
using Infrastructure.Domain;
using MediatR;

namespace Accounts.Application.Handlers
{
    public sealed class AccountsCommandHandler :
        IRequestHandler<OpenAccount, Guid>,
        IRequestHandler<DepositToAccount>,
        IRequestHandler<WithdrawFromAccount>,
        IRequestHandler<AddInterestsToAccount>,
        IRequestHandler<FreezeAccount>,
        IRequestHandler<UnFreezeAccount>
    {
        private readonly IEventSourcedRepository<Account> _repository;

        public AccountsCommandHandler(IEventSourcedRepository<Account> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(OpenAccount command, CancellationToken token)
        {
            var id = DeterministicGuid.Create(command.Id);
            var account = Account.Open(id, command.ClientId, (InterestRate)command.InterestRate, command.Balance);
            await _repository.CreateAsync(account, command.Id);
            return id;
        }

        public async Task<Unit> Handle(DepositToAccount command, CancellationToken token)
        {
            await _repository.ExecuteAsync(command.AccountId, account => account.Deposit(command.Amount), command.Id, command.Id);
            return default;
        }

        public async Task<Unit> Handle(WithdrawFromAccount command, CancellationToken token)
        {
            await _repository.ExecuteAsync(command.AccountId, account => account.Withdraw(command.Amount), command.Id, command.Id);
            return default;
        }

        public async Task<Unit> Handle(AddInterestsToAccount command, CancellationToken token)
        {
            await _repository.ExecuteAsync(command.AccountId, account => account.AddInterests(), command.Id, command.Id);
            return default;
        }

        public async Task<Unit> Handle(FreezeAccount command, CancellationToken token)
        {
            await _repository.ExecuteAsync(command.AccountId, account => account.Freeze(), command.Id);
            return default;
        }

        public async Task<Unit> Handle(UnFreezeAccount command, CancellationToken token)
        {
            await _repository.ExecuteAsync(command.AccountId, account => account.Unfreeze(), command.Id);
            return default;
        }
    }
}