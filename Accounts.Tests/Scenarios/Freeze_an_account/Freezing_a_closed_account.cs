namespace Accounts.Tests.Scenarios.Freeze_an_account;

public sealed class Freezing_a_closed_account : AccountSpecification<FreezeAccount>
{
    protected override IEnumerable<IEvent> Given()
    {
        yield return new AccountOpened(AccountId, Guid.NewGuid(), 0, 0);
        yield return new AccountClosed(AccountId);
    }

    protected override FreezeAccount When()
    {
        return new(AccountId);
    }

    protected override IEnumerable<IEvent> Then()
    {
        throw new ClosedAccountException();
    }
}
