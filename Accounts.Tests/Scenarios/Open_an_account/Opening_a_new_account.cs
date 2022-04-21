namespace Accounts.Tests.Scenarios.Open_an_account;

public sealed class Opening_a_new_account : AccountSpecification<OpenAccount>
{
    private readonly ClientId _clientId = new(Guid.NewGuid());

    protected override OpenAccount When()
    {
        return new(_clientId, new InterestRate(0));
    }

    protected override IEnumerable<IEvent> Then()
    {
        yield return new AccountOpened(AccountId, _clientId, 0);
    }
}
