using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Exceptions;
using Accounts.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Accounts.ReadModel
{
    public sealed class AccountReadModelGenerator
    {
        private readonly DbContextOptions<AccountDbContext> _ctxOptions;

        public AccountReadModelGenerator(DbContextOptions<AccountDbContext> ctxOptions)
        {
            _ctxOptions = ctxOptions;
        }

        public async Task Handle(AccountOpened evt, CancellationToken token = default)
        {
            await using var ctx = new AccountDbContext(_ctxOptions);
            var account = await ctx.Accounts.FindAsync(new object[] { evt.SourceId }, token);
            if (account != null)
                return;

            account = new ActiveAccount
            {
                Id = evt.SourceId,
                Version = evt.Version,
                ClientId = evt.ClientId,
                Balance = evt.Balance,
                InterestRate = evt.InterestRate,
                IsFrozen = false
            };

            ctx.Accounts.Add(account);
            await ctx.SaveChangesAsync(token);
        }

        public async Task Handle(WithdrawnFromAccount evt, CancellationToken token = default)
        {
            await UpdateAsync(
                evt.SourceId,
                evt.Version,
                account => account.Balance -= evt.Amount,
                token);
        }

        public async Task Handle(DepositedToAccount evt, CancellationToken token = default)
        {
            await UpdateAsync(
                evt.SourceId,
                evt.Version,
                account => account.Balance += evt.Amount,
                token);
        }

        public async Task Handle(AddedInterestsToAccount evt, CancellationToken token = default)
        {
            await UpdateAsync(
                evt.SourceId,
                evt.Version,
                account => account.Balance += evt.Interests,
                token);
        }

        public async Task Handle(AccountFrozen evt, CancellationToken token = default)
        {
            await UpdateAsync(
                evt.SourceId,
                evt.Version,
                account => account.IsFrozen = true,
                token);
        }

        public async Task Handle(AccountUnFrozen evt, CancellationToken token = default)
        {
            await UpdateAsync(
                evt.SourceId,
                evt.Version,
                account => account.IsFrozen = false,
                token);
        }

        private async Task UpdateAsync(Guid id, int version, Action<ActiveAccount> action, CancellationToken token = default)
        {
            await using var ctx = new AccountDbContext(_ctxOptions);
            var account = await ctx.Accounts.FindAsync(new object[] { id }, token) ??
                          throw new EntityNotFoundException(typeof(ActiveAccount).Name, id);

            if (version <= account.Version)
                return;

            action(account);
            account.Version = version;

            ctx.Update(account);
            await ctx.SaveChangesAsync(token);
        }
    }
}