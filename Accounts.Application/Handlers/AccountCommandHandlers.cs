using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Common;
using Accounts.Domain;
using MediatR;

namespace Accounts.Application.Handlers
{
    public sealed class AccountCommandHandlers :
        IRequestHandler<OpenAccount, Guid>,
        IRequestHandler<DepositToAccount>,
        IRequestHandler<WithdrawFromAccount>,
        IRequestHandler<AddInterestsToAccount>,
        IRequestHandler<FreezeAccount>,
        IRequestHandler<UnfreezeAccount>
    {
        private readonly IAccountRepository _repository;

        public AccountCommandHandlers(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(OpenAccount command, CancellationToken token = default)
        {
            var id = _repository.GetNextIdentity();
            var account = Account.Open(id, command.ClientId, new(command.InterestRate), command.Balance);
            await _repository.SaveAsync(account, token);
            return id;
        }

        public async Task<Unit> Handle(DepositToAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(new(command.AccountId), account => account.Deposit(command.Amount), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(WithdrawFromAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(new(command.AccountId), account => account.Withdraw(command.Amount), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(AddInterestsToAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(new(command.AccountId), account => account.AddInterests(), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(FreezeAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(new(command.AccountId), account => account.Freeze(), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UnfreezeAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(new(command.AccountId), account => account.Unfreeze(), token);
            return Unit.Value;
        }
    }
}