namespace Accounts.Domain;

public sealed class FrozenAccountException : DomainException
{
    public FrozenAccountException()
        : base("Account is frozen.") { }
}

public sealed class ClosedAccountException : DomainException
{
    public ClosedAccountException()
        : base("Account is closed.") { }
}

public sealed class InsufficientBalanceException : DomainException
{
    public InsufficientBalanceException()
        : base("Insufficient balance.") { }
}
