using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain.Events;
using Infrastructure.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Accounts.ReadModel.Configurations.AccountConfig;

namespace Accounts.ReadModel
{
    public sealed class AccountReadModelGenerator
    {
        private readonly NumberFormatInfo _numberFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };
        private readonly DbContextOptions<AccountDbContext> _contextOptions;

        public AccountReadModelGenerator(DbContextOptions<AccountDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
        }

        public async Task Handle(AccountOpened @event, CancellationToken token = default)
        {
            var account = new ActiveAccount
            {
                Id = @event.SourceId,
                Version = @event.Version,
                ClientId = @event.ClientId,
                Balance = @event.Balance,
                InterestRate = @event.InterestRate,
                IsFrozen = false
            };

            await using var context = new AccountDbContext(_contextOptions);
            context.Accounts.Add(account);

            try
            {
                await context.SaveChangesAsync(token);
            }
            catch (DbUpdateException e)
                when (e.GetBaseException() is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                // Ignore duplicate key.
            }
        }

        public async Task Handle(WithdrawnFromAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event, $"{BalanceColumnName} -= {@event.Amount.ToString(_numberFormat)}", token);
        }

        public async Task Handle(DepositedToAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event, $"{BalanceColumnName} += {@event.Amount.ToString(_numberFormat)}", token);
        }

        public async Task Handle(AddedInterestsToAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event, $"{BalanceColumnName} += {@event.Interests.ToString(_numberFormat)}", token);
        }

        public async Task Handle(AccountFrozen @event, CancellationToken token = default)
        {
            await UpdateAsync(@event, $"{IsFrozenColumnName} = 1", token);
        }

        public async Task Handle(AccountUnFrozen @event, CancellationToken token = default)
        {
            await UpdateAsync(@event, $"{IsFrozenColumnName} = 0", token);
        }

        private async Task UpdateAsync<TEvent>(TEvent @event, string updateSql, CancellationToken token)
            where TEvent : IVersionedEvent
        {
            var sql = $@"
                UPDATE {TableName} 
                SET {updateSql} 
                WHERE {IdColumnName} = '{@event.SourceId}' 
                  AND {VersionColumnName} < {@event.Version}";

            await using var context = new AccountDbContext(_contextOptions);
            await context.Database.ExecuteSqlRawAsync(sql, token);
        }
    }
}