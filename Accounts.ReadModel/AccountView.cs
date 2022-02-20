namespace Accounts.ReadModel;

public sealed class AccountView
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public AccountView(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task HandleAsync(AccountOpened @event)
    {
        await using var connection = _connectionFactory.Create();

        try
        {
            await connection.ExecuteAsync(@"
                INSERT INTO Accounts
                (
                    Id,
                    Version,
                    ClientId,
                    InterestRate,
                    Balance,
                    IsFrozen
                )
                VALUES
                (
                    @Id,
                    @Version,
                    @ClientId,
                    @InterestRate,
                    @Balance,
                    @IsFrozen
                )",
                new AccountDto
                {
                    Id = @event.AccountId,
                    Version = 0,
                    ClientId = @event.ClientId,
                    InterestRate = @event.InterestRate,
                    Balance = @event.Balance,
                    IsFrozen = false
                });
        }
        catch (MySqlException e)
            when (e.Number is 2627 or 2601)
        {
            // Ignore duplicate key.
        }
    }

    public async Task HandleAsync(WithdrawnFromAccount @event, int version)
    {
        await UpdateBalanceAsync(@event.AccountId, -@event.Amount, version);
    }

    public async Task HandleAsync(DepositedToAccount @event, int version)
    {
        await UpdateBalanceAsync(@event.AccountId, @event.Amount, version);
    }

    public async Task HandleAsync(AddedInterestsToAccount @event, int version)
    {
        await UpdateBalanceAsync(@event.AccountId, @event.Interests, version);
    }

    public async Task HandleAsync(AccountFrozen @event, int version)
    {
        await UpdateFrozenFlagAsync(@event.AccountId, true, version);
    }

    public async Task HandleAsync(AccountUnfrozen @event, int version)
    {
        await UpdateFrozenFlagAsync(@event.AccountId, false, version);
    }

    public async Task HandleAsync(AccountClosed @event)
    {
        await using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(@"
            DELETE
            FROM Accounts 
            WHERE Id = @Id",
            new
            {
                Id = @event.AccountId
            });
    }

    private async Task UpdateBalanceAsync(Guid accountId, decimal balanceDelta, int version)
    {
        await using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(@"
            UPDATE Accounts 
            SET 
                Balance = Balance + @BalanceDelta,
                Version = @Version
            WHERE 
                Id = @Id AND 
                Version < @Version",
            new
            {
                Id = accountId,
                BalanceDelta = balanceDelta,
                Version = version
            });
    }

    private async Task UpdateFrozenFlagAsync(Guid accountId, bool isFrozen, int version)
    {
        await using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(@"
            UPDATE Accounts 
            SET 
                IsFrozen = @IsFrozen,
                Version = @Version
            WHERE 
                Id = @Id AND 
                Version < @Version",
            new
            {
                Id = accountId,
                IsFrozen = isFrozen,
                Version = version
            });
    }
}
