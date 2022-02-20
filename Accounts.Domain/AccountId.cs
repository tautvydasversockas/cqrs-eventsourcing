namespace Accounts.Domain;

public sealed class AccountId : SimpleValueObject<Guid>
{
    public AccountId(Guid value)
        : base(value) { }
}
