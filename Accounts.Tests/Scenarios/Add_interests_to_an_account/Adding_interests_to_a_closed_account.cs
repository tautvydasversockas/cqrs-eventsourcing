namespace Accounts.Tests.Scenarios.Add_interests_to_an_account;

public sealed class Adding_interests_to_a_closed_account : AccountSpecification<AddInterestsToAccount>
{
    protected override IEnumerable<IEvent> Given()
    {
        yield return new AccountOpened(AccountId, Guid.NewGuid(), 0);
        yield return new AccountClosed(AccountId);
    }

    protected override AddInterestsToAccount When()
    {
        return new(AccountId);
    }

    protected override IEnumerable<IEvent> Then()
    {
        throw new ClosedAccountException();
    }
}
