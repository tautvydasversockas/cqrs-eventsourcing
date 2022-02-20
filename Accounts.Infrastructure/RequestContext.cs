namespace Accounts.Infrastructure;

public static class RequestContext
{
    private static readonly AsyncLocal<Guid?> RequestIdAsyncLocal = new();
    public static Guid? RequestId
    {
        get => RequestIdAsyncLocal.Value;
        set => RequestIdAsyncLocal.Value = value;
    }

    private static readonly AsyncLocal<Guid?> CausationIdAsyncLocal = new();
    public static Guid? CausationId
    {
        get => CausationIdAsyncLocal.Value;
        set => CausationIdAsyncLocal.Value = value;
    }

    private static readonly AsyncLocal<Guid?> CorrelationIdAsyncLocal = new();
    public static Guid? CorrelationId
    {
        get => CorrelationIdAsyncLocal.Value;
        set => CorrelationIdAsyncLocal.Value = value;
    }
}
