namespace Accounts.Tests.Scenarios.Deposit_to_an_account;

public sealed class Depositing_to_a_frozen_account : AccountSpecification<DepositToAccount>
{
    protected override IEnumerable<IEvent> Given()
    {
        yield return new AccountOpened(AccountId, Guid.NewGuid(), 0);
        yield return new AccountFrozen(AccountId);
    }

    protected override DepositToAccount When()
    {
        return new(AccountId, new Money(100));
    }

    protected override IEnumerable<IEvent> Then()
    {
        throw new FrozenAccountException();
    }
}
