namespace Accounts.Tests.Scenarios.Deposit_to_an_account;

public sealed class Depositing_to_an_active_account : AccountSpecification<DepositToAccount>
{
    protected override IEnumerable<IEvent> Given()
    {
        yield return new AccountOpened(AccountId, Guid.NewGuid(), 0);
    }

    protected override DepositToAccount When()
    {
        return new(AccountId, new Money(100));
    }

    protected override IEnumerable<IEvent> Then()
    {
        yield return new DepositedToAccount(AccountId, 100);
    }
}
