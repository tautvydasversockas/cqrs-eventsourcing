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
        private readonly IEventSourcedRepository<Account, Guid> _repository;

        public AccountsCommandHandler(IEventSourcedRepository<Account, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(OpenAccount cmd, CancellationToken token)
        {
            var id = DeterministicGuid.Create(cmd.Id);
            var account = Account.Open(id, cmd.ClientId, cmd.InterestRate, cmd.Balance);
            await _repository.CreateAsync(account, cmd.Id.ToString());
            return id;
        }

        public async Task<Unit> Handle(DepositToAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Deposit(cmd.Amount), cmd.Id.ToString(), cmd.Id);
            return default;
        }

        public async Task<Unit> Handle(WithdrawFromAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Withdraw(cmd.Amount), cmd.Id.ToString(), cmd.Id);
            return default;
        }

        public async Task<Unit> Handle(AddInterestsToAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.AddInterests(), cmd.Id.ToString(), cmd.Id);
            return default;
        }

        public async Task<Unit> Handle(FreezeAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Freeze(), cmd.Id.ToString());
            return default;
        }

        public async Task<Unit> Handle(UnFreezeAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Unfreeze(), cmd.Id.ToString());
            return default;
        }
    }
}
