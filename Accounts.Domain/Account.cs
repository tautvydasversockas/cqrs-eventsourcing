namespace Accounts.Domain;

public sealed class Account : EventSourcedAggregate<AccountId>
{
    private AccountStatus _status;
    private decimal _balance;
    private InterestRate _interestRate;

    private Account() 
    {
        _status = null!;
        _balance = 0;
        _interestRate = null!;
    }

    public static Account Open(
        AccountId id,
        ClientId clientId,
        InterestRate interestRate)
    {
        var account = new Account();
        account.Raise(new AccountOpened(id, clientId, interestRate));
        return account;
    }

    public void Withdraw(Money money)
    {
        ThrowIfInactive();

        if (money > _balance)
            throw new InsufficientBalanceException();

        Raise(new WithdrawnFromAccount(Id, money));
    }

    public void Deposit(Money money)
    {
        ThrowIfInactive();

        Raise(new DepositedToAccount(Id, money));
    }

    public void AddInterests()
    {
        ThrowIfInactive();

        var interests = _balance * _interestRate;

        Raise(new AddedInterestsToAccount(Id, interests));
    }

    public void Freeze()
    {
        ThrowIfClosed();

        if (_status != Frozen)
            Raise(new AccountFrozen(Id));
    }

    public void Unfreeze()
    {
        ThrowIfClosed();

        if (_status != Active)
            Raise(new AccountUnfrozen(Id));
    }

    public void Close()
    {
        if (_status != Closed)
            Raise(new AccountClosed(Id));
    }

    private void ThrowIfInactive()
    {
        ThrowIfClosed();
        ThrowIfFrozen();
    }

    private void ThrowIfClosed()
    {
        if (_status == Closed)
            throw new ClosedAccountException();
    }

    private void ThrowIfFrozen()
    {
        if (_status == Frozen)
            throw new FrozenAccountException();
    }

    private void Apply(AccountOpened @event)
    {
        Id = new AccountId(@event.AccountId);
        _status = Active;
        _balance = 0;
        _interestRate = new InterestRate(@event.InterestRate);
    }

    private void Apply(WithdrawnFromAccount @event)
    {
        _balance -= @event.Amount;
    }

    private void Apply(DepositedToAccount @event)
    {
        _balance += @event.Amount;
    }

    private void Apply(AddedInterestsToAccount @event)
    {
        _balance += @event.Interests;
    }

    private void Apply(AccountFrozen @event)
    {
        _status = Frozen;
    }

    private void Apply(AccountUnfrozen @event)
    {
        _status = Active;
    }

    private void Apply(AccountClosed @event)
    {
        _status = Closed;
    }
}
