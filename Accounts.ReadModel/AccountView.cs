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
            var account = new AccountDto(
                Id: @event.AccountId,
                Version: 0,
                ClientId: @event.ClientId,
                InterestRate: @event.InterestRate,
                Balance: @event.Balance,
                IsFrozen: false);

            _context.Accounts.Add(account);

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

        public async Task HandleAsync(WithdrawnFromAccount @event, int version, CancellationToken token = default)
        {
            await _context.Database.ExecuteSqlRawAsync($@"
                UPDATE Accounts 
                SET Balance -= {ParseDecimal(@event.Amount)},
                    Version = {version}
                WHERE Id = '{@event.AccountId}'
                  AND Version < {version}",
                token);
        }

        public async Task HandleAsync(DepositedToAccount @event, int version, CancellationToken token = default)
        {
            await _context.Database.ExecuteSqlRawAsync($@"
                UPDATE Accounts 
                SET Balance -= {ParseDecimal(@event.Amount)},
                    Version = {version}
                WHERE Id = '{@event.AccountId}'
                  AND Version < {version}",
                token);
        }

        public async Task HandleAsync(AddedInterestsToAccount @event, int version, CancellationToken token = default)
        {
            await _context.Database.ExecuteSqlRawAsync($@"
                UPDATE Accounts 
                SET Balance += {ParseDecimal(@event.Interests)},
                    Version = {version}
                WHERE Id = '{@event.AccountId}'
                  AND Version < {version}",
                token);
        }

        public async Task HandleAsync(AccountFrozen @event, int version, CancellationToken token = default)
        {
            await _context.Database.ExecuteSqlRawAsync($@"
                UPDATE Accounts 
                SET IsFrozen = 1,
                    Version = {version}
                WHERE Id = '{@event.AccountId}'
                  AND Version < {version}",
                token);
        }

        public async Task HandleAsync(AccountUnfrozen @event, int version, CancellationToken token = default)
        {
            await _context.Database.ExecuteSqlRawAsync($@"
                UPDATE Accounts 
                SET IsFrozen = 0,
                    Version = {version}
                WHERE Id = '{@event.AccountId}'
                  AND Version < {version}",
                token);
        }

        public async Task HandleAsync(AccountClosed @event, CancellationToken token = default)
        {
            await _context.Database.ExecuteSqlRawAsync($@"
                DELETE
                FROM Accounts 
                WHERE Id = '{@event.AccountId}'",
                token);
        }

        private static string ParseDecimal(decimal value) =>
            value.ToString(NumberFormat);
    }
}