using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Accounts.ReadModel
{
    public sealed class AccountView
    {
        private static readonly NumberFormatInfo NumberFormat = new()
        {
            NumberDecimalSeparator = "."
        };

        private readonly AccountDbContext _context;

        public AccountView(AccountDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(AccountOpened @event, CancellationToken token = default)
        {
            _context.Accounts.Add(new(
                Id: @event.AccountId,
                Version: @event.Version,
                ClientId: @event.ClientId,
                InterestRate: @event.Balance,
                Balance: @event.InterestRate,
                IsFrozen: false));

            try
            {
                await _context.SaveChangesAsync(token);
            }
            catch (DbUpdateException e)
                when (e.GetBaseException() is SqlException sqlEx && sqlEx.Number is 2627 or 2601)
            {
                // Ignore duplicate key.
            }
        }

        public async Task HandleAsync(WithdrawnFromAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, $"Balance -= {ParseDecimal(@event.Amount)}", token);
        }

        public async Task HandleAsync(DepositedToAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, $"Balance += {ParseDecimal(@event.Amount)}", token);
        }

        public async Task HandleAsync(AddedInterestsToAccount @event, CancellationToken token = default)
        {
            await UpdateAsync(@event.AccountId, @event.Version, $"Balance += {ParseDecimal(@event.Interests)}", token);
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
            return _context.Database.ExecuteSqlRawAsync($@"
                UPDATE Accounts 
                SET {updateSql} 
                WHERE Id = '{id}' 
                  AND Version < {version}",
                token);
        }

        private static string ParseDecimal(decimal value) =>
            value.ToString(NumberFormat);
    }
}