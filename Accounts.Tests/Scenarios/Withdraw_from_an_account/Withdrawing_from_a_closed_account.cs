namespace Accounts.Tests.Scenarios.Withdraw_from_an_account;

public sealed class Withdrawing_from_a_closed_account : AccountSpecification<WithdrawFromAccount>
{
    protected override IEnumerable<IEvent> Given()
    {
        yield return new AccountOpened(AccountId, Guid.NewGuid(), 0);
        yield return new AccountClosed(AccountId);
    }

    protected override WithdrawFromAccount When()
    {
        return new(AccountId, new Money(100));
    }

    protected override IEnumerable<IEvent> Then()
    {
        throw new ClosedAccountException();
    }
}
