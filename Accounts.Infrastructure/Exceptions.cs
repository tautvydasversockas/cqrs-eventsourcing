namespace Accounts.Infrastructure;

public sealed class DuplicateKeyException : Exception
{
    public DuplicateKeyException(object key)
        : base($"Duplicate key ({key}).") { }
}

public sealed class DuplicateRequestException : Exception
{
    public DuplicateRequestException(object requestId)
        : base($"Duplicate request ({requestId}).") { }
}
