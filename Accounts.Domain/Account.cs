namespace Accounts.Domain;

public sealed class Account : EventSourcedAggregate<AccountId>
{
    public enum Status
    {
        Active,
        Frozen,
        Closed
    }

    private Status _status;
    private decimal _balance;
    private InterestRate _interestRate = null!;

    private Account() { }

    public static Account Open(
        AccountId id,
        ClientId clientId,
        InterestRate interestRate,
        decimal balance)
    {
        if (balance < 0)
            throw new ArgumentOutOfRangeException(nameof(balance), "Balance can't be negative.");

        var account = new Account();
        account.Raise(new AccountOpened(id, clientId, interestRate, balance));
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

        if (_status is not Frozen)
            Raise(new AccountFrozen(Id));
    }

    public void Unfreeze()
    {
        ThrowIfClosed();

        if (_status is not Active)
            Raise(new AccountUnfrozen(Id));
    }

    public void Close()
    {
        if (_status is not Closed)
            Raise(new AccountClosed(Id));
    }

    private void ThrowIfInactive()
    {
        ThrowIfClosed();
        ThrowIfFrozen();
    }

    private void ThrowIfClosed()
    {
        if (_status is Closed)
            throw new ClosedAccountException();
    }

    private void ThrowIfFrozen()
    {
        if (_status is Frozen)
            throw new FrozenAccountException();
    }

    private void Apply(AccountOpened @event)
    {
        Id = new AccountId(@event.AccountId);
        _status = Active;
        _balance = @event.Balance;
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
