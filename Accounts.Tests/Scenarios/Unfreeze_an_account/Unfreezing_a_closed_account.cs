namespace Accounts.Tests.Scenarios.Unfreeze_an_account;

public sealed class Unfreezing_a_closed_account : AccountSpecification<UnfreezeAccount>
{
    protected override IEnumerable<IEvent> Given()
    {
        yield return new AccountOpened(AccountId, Guid.NewGuid(), 0, 0);
        yield return new AccountClosed(AccountId);
    }

    protected override UnfreezeAccount When()
    {
        return new(AccountId);
    }

    protected override IEnumerable<IEvent> Then()
    {
        throw new ClosedAccountException();
    }
}
