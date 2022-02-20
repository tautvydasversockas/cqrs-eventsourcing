namespace Accounts.Infrastructure.HealthChecks;

public static class BackgroundServiceStatistics
{
    private static readonly object LockObject = new();
    private static DateTimeOffset _lastProcessTime;

    public static DateTimeOffset LastProcessTime
    {
        get
        {
            lock (LockObject)
                return _lastProcessTime;
        }
    }

    public static void SetLastProcessTime()
    {
        lock (LockObject)
            _lastProcessTime = DateTimeOffset.Now;
    }
}
