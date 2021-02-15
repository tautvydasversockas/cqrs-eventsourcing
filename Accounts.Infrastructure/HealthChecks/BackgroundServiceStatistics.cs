using System;

namespace Accounts.Infrastructure.HealthChecks
{
    public static class BackgroundServiceStatistics
    {
        private static readonly object LockObject = new();
        private static DateTime _lastProcessTime;

        public static DateTime LastProcessTime
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
                _lastProcessTime = DateTime.UtcNow;
        }
    }
}