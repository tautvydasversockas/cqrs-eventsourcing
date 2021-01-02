using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Common;
using Accounts.Domain;
using Accounts.Domain.Common;
using MediatR;

namespace Accounts.Application.Handlers
{
    public sealed class AccountCommandHandlers :
        EventSourcedAggregateHandler<Account, Guid>,
        IRequestHandler<OpenAccount>,
        IRequestHandler<DepositToAccount>,
        IRequestHandler<WithdrawFromAccount>,
        IRequestHandler<AddInterestsToAccount>,
        IRequestHandler<FreezeAccount>,
        IRequestHandler<UnfreezeAccount>
    {
        public AccountCommandHandlers(IEventSourcedRepository<Account, Guid> repository)
            : base(repository) { }

        public async Task<Unit> Handle(OpenAccount command, CancellationToken token = default)
        {
            var account = Account.Open(command.AccountId, command.ClientId, new(command.InterestRate), command.Balance);
            await CreateAsync(account, token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DepositToAccount command, CancellationToken token = default)
        {
            await UpdateAsync(command.AccountId, account => account.Deposit(command.Amount), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(WithdrawFromAccount command, CancellationToken token = default)
        {
            await UpdateAsync(command.AccountId, account => account.Withdraw(command.Amount), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(AddInterestsToAccount command, CancellationToken token = default)
        {
            await UpdateAsync(command.AccountId, account => account.AddInterests(), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(FreezeAccount command, CancellationToken token = default)
        {
            await UpdateAsync(command.AccountId, account => account.Freeze(), token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UnfreezeAccount command, CancellationToken token = default)
        {
            await UpdateAsync(command.AccountId, account => account.Unfreeze(), token);
            return Unit.Value;
        }
    }
}