namespace Accounts.Tests.Scenarios.Add_interests_to_an_account;

public sealed class Adding_interests_to_an_active_account : AccountSpecification<AddInterestsToAccount>
{
    protected override IEnumerable<IEvent> Given()
    {
        yield return new AccountOpened(AccountId, Guid.NewGuid(), 0.1m);
        yield return new DepositedToAccount(AccountId, 100);
    }

    protected override AddInterestsToAccount When()
    {
        return new(AccountId);
    }

    protected override IEnumerable<IEvent> Then()
    {
        yield return new AddedInterestsToAccount(AccountId, 10);
    }
}
