using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain.Events;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Accounts.ReadModel
{
    public sealed class AccountView
    {
        private static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };
        private readonly AccountDbContext _context;

        public AccountView(AccountDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(AccountOpened @event, CancellationToken token = default)
        {
            var account = new AccountDto
            {
                Id = @event.AccountId,
                Version = @event.Version,
                ClientId = @event.ClientId,
                Balance = @event.Balance,
                InterestRate = @event.InterestRate,
                IsFrozen = false
            };

            _context.Accounts.Add(account);

            try
            {
                await _context.SaveChangesAsync(token);
            }
            catch (DbUpdateException e)
                when (e.GetBaseException() is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                // Ignore duplicate key.
            }
        }

        public async Task HandleAsync(WithdrawnFromAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, $"Balance -= {@event.Amount.ToString(NumberFormat)}", token);
        }

        public async Task HandleAsync(DepositedToAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, $"Balance += {@event.Amount.ToString(NumberFormat)}", token);
        }

        public async Task HandleAsync(AddedInterestsToAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, $"Balance += {@event.Interests.ToString(NumberFormat)}", token);
        }

        public async Task HandleAsync(AccountFrozen @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, "IsFrozen = 1", token);
        }

        public async Task HandleAsync(AccountUnfrozen @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, "IsFrozen = 0", token);
        }

        private Task UpdateAsync(Guid id, int version, string updateSql, CancellationToken token)
        {
            var sql = $@"
                UPDATE Accounts 
                SET {updateSql} 
                WHERE Id = '{id}' 
                  AND Version < {version}";
            return _context.Database.ExecuteSqlRawAsync(sql, token);
        }
    }
}