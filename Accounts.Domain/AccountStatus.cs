namespace Accounts.Domain;

public sealed class AccountStatus : SimpleValueObject<string>
{
    public static readonly AccountStatus Active = new("Active");
    public static readonly AccountStatus Frozen = new("Frozen");
    public static readonly AccountStatus Closed = new("Closed");

    private AccountStatus(string value)
        : base(value) { }
}
