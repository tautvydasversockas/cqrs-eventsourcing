namespace Accounts.Domain;

public abstract class DomainException : Exception
{
    protected DomainException(string message)
        : base(message) { }
}

public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string name, object key)
        : base($"Entity '{name}' ({key}) was not found.") { }
}

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
