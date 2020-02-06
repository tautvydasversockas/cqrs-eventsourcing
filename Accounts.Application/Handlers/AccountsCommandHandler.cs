using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Commands;
using Accounts.Application.Common;
using Accounts.Domain;
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
            var account = Account.Open(Guid.NewGuid(), cmd.ClientId, cmd.InterestRate, cmd.Balance);
            await _repository.AddAsync(account);
            return account.Id;
        }

        public async Task<Unit> Handle(DepositToAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Deposit(cmd.Amount));
            return Unit.Value;
        }

        public async Task<Unit> Handle(WithdrawFromAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Withdraw(cmd.Amount));
            return Unit.Value;
        }

        public async Task<Unit> Handle(AddInterestsToAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.AddInterests());
            return Unit.Value;
        }

        public async Task<Unit> Handle(FreezeAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Freeze());
            return Unit.Value;
        }

        public async Task<Unit> Handle(UnFreezeAccount cmd, CancellationToken token)
        {
            await _repository.ExecuteAsync(cmd.AccountId, account => account.Unfreeze());
            return Unit.Value;
        }
    }
}