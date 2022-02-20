namespace Accounts.ReadModel;

public sealed class AccountReadModel
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public AccountReadModel(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsAsync()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<AccountDto>(@"
            SELECT *
            FROM Accounts");
    }

    public async Task<AccountDto?> GetAccountByIdAsync(Guid id)
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<AccountDto?>(@"
            SELECT *
            FROM Accounts
            WHERE Id = @Id",
            new
            {
                Id = id
            });
    }
}
