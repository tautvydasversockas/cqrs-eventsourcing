using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Commands;
using Accounts.Application.Exceptions;
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
            var account = await _repository.GetAsync(cmd.Id);
            if (account != null)
                return account.Id;

            account = Account.Open(cmd.Id, cmd.ClientId, cmd.InterestRate, cmd.Balance);
            await _repository.SaveAsync(account, cmd.Id.ToString());
            return account.Id;
        }

        public async Task<Unit> Handle(DepositToAccount cmd, CancellationToken token)
        {
            var account = await _repository.GetAsync(cmd.AccountId) ??
                            throw new EntityNotFoundException(typeof(Account).Name, cmd.AccountId);

            try
            {
                account.SetOperation(cmd.Id);
            }
            catch (InvalidOperationException)
            {
                return Unit.Value;
            }

            account.Deposit(cmd.Amount);

            await _repository.SaveAsync(account, cmd.Id.ToString());
            return Unit.Value;
        }

        public async Task<Unit> Handle(WithdrawFromAccount cmd, CancellationToken token)
        {
            var account = await _repository.GetAsync(cmd.AccountId) ??
                          throw new EntityNotFoundException(typeof(Account).Name, cmd.AccountId);

            try
            {
                account.SetOperation(cmd.Id);
            }
            catch (InvalidOperationException)
            {
                return Unit.Value;
            }

            account.Withdraw(cmd.Amount);

            await _repository.SaveAsync(account, cmd.Id.ToString());
            return Unit.Value;
        }

        public async Task<Unit> Handle(AddInterestsToAccount cmd, CancellationToken token)
        {
            var account = await _repository.GetAsync(cmd.AccountId) ??
                          throw new EntityNotFoundException(typeof(Account).Name, cmd.AccountId);

            try
            {
                account.SetOperation(cmd.Id);
            }
            catch (InvalidOperationException)
            {
                return Unit.Value;
            }

            account.AddInterests();

            await _repository.SaveAsync(account, cmd.Id.ToString());
            return Unit.Value;
        }

        public async Task<Unit> Handle(FreezeAccount cmd, CancellationToken token)
        {
            var account = await _repository.GetAsync(cmd.AccountId) ??
                          throw new EntityNotFoundException(typeof(Account).Name, cmd.AccountId);

            account.Freeze();

            await _repository.SaveAsync(account, cmd.Id.ToString());
            return Unit.Value;
        }

        public async Task<Unit> Handle(UnFreezeAccount cmd, CancellationToken token)
        {
            var account = await _repository.GetAsync(cmd.AccountId) ??
                          throw new EntityNotFoundException(typeof(Account).Name, cmd.AccountId);

            account.Unfreeze();

            await _repository.SaveAsync(account, cmd.Id.ToString());
            return Unit.Value;
        }
    }
}