﻿using Accounts.Application.Common;
using Accounts.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Application.Handlers
{
    public sealed class AccountCommandHandlers :
        IRequestHandler<OpenAccount, Guid>,
        IRequestHandler<DepositToAccount>,
        IRequestHandler<WithdrawFromAccount>,
        IRequestHandler<AddInterestsToAccount>,
        IRequestHandler<FreezeAccount>,
        IRequestHandler<UnfreezeAccount>,
        IRequestHandler<CloseAccount>
    {
        private readonly IAccountRepository _repository;

        public AccountCommandHandlers(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(OpenAccount command, CancellationToken token = default)
        {
            var account = Account.Open(
                _repository.GetNextIdentity(),
                new ClientId(command.ClientId),
                new InterestRate(command.InterestRate),
                command.Balance);
            await _repository.SaveAsync(account, token);
            return account.Id;
        }

        public async Task<Unit> Handle(DepositToAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(
                new AccountId(command.AccountId),
                account => account.Deposit(new Money(command.Amount)),
                token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(WithdrawFromAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(
                new AccountId(command.AccountId),
                account => account.Withdraw(new Money(command.Amount)),
                token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(AddInterestsToAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(
                new AccountId(command.AccountId),
                account => account.AddInterests(),
                token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(FreezeAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(
                new AccountId(command.AccountId),
                account => account.Freeze(),
                token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UnfreezeAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(
                new AccountId(command.AccountId),
                account => account.Unfreeze(),
                token);
            return Unit.Value;
        }

        public async Task<Unit> Handle(CloseAccount command, CancellationToken token = default)
        {
            await _repository.UpdateAsync(
                new AccountId(command.AccountId),
                account => account.Close(),
                token);
            return Unit.Value;
        }
    }
}