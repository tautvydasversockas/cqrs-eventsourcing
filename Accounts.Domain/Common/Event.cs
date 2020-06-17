namespace Accounts.Domain.Common
{
    public abstract class Event : Message
    {
        public int Version { get; set; }
    }
}