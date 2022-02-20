namespace Accounts.ReadModel;

public sealed class MySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public MySqlConnection Create()
    {
        return new(_connectionString);
    }
}
