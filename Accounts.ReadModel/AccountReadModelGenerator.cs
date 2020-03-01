using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain.Events;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Accounts.ReadModel.Configurations.AccountConfig;

namespace Accounts.ReadModel
{
    public sealed class AccountReadModelGenerator
    {
        private readonly NumberFormatInfo _numberFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };
        private readonly DbContextOptions<AccountDbContext> _ctxOptions;

        public AccountReadModelGenerator(DbContextOptions<AccountDbContext> ctxOptions)
        {
            _ctxOptions = ctxOptions;
        }

        public async Task Handle(AccountOpened evt, CancellationToken token = default)
        {
            await using var ctx = new AccountDbContext(_ctxOptions);

            ctx.Accounts.Add(new ActiveAccount
            {
                Id = evt.SourceId,
                Version = evt.Version,
                ClientId = evt.ClientId,
                Balance = evt.Balance,
                InterestRate = evt.InterestRate,
                IsFrozen = false
            });

            try
            {
                await ctx.SaveChangesAsync(token);
            }
            catch (DbUpdateException e)
                when (e.GetBaseException() is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                //Ignore duplicate key
            }
        }

        public async Task Handle(WithdrawnFromAccount evt, CancellationToken token = default)
        {
            await UpdateAsync(evt.SourceId, evt.Version, $"{BalanceColumnName} -= {evt.Amount.ToString(_numberFormat)}", token);
        }

        public async Task Handle(DepositedToAccount evt, CancellationToken token = default)
        {
            await UpdateAsync(evt.SourceId, evt.Version, $"{BalanceColumnName} += {evt.Amount.ToString(_numberFormat)}", token);
        }

        public async Task Handle(AddedInterestsToAccount evt, CancellationToken token = default)
        {
            await UpdateAsync(evt.SourceId, evt.Version, $"{BalanceColumnName} += {evt.Interests.ToString(_numberFormat)}", token);
        }

        public async Task Handle(AccountFrozen evt, CancellationToken token = default)
        {
            await UpdateAsync(evt.SourceId, evt.Version, $"{IsFrozenColumnName} = 1", token);
        }

        public async Task Handle(AccountUnFrozen evt, CancellationToken token = default)
        {
            await UpdateAsync(evt.SourceId, evt.Version, $"{IsFrozenColumnName} = 0", token);
        }

        private async Task UpdateAsync(Guid id, int version, string updateSql, CancellationToken token)
        {
            await using var ctx = new AccountDbContext(_ctxOptions);
            await ctx.Database.ExecuteSqlRawAsync($@"UPDATE {TableName} SET {updateSql} 
                WHERE {IdColumnName} = '{id}' AND {VersionColumnName} < {version}", token);
        }
    }
}