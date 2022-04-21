namespace Accounts.Domain.Common;

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
