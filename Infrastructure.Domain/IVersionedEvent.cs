namespace Infrastructure.Domain
{
    public interface IVersionedEvent<out TSourceId> : IEvent<TSourceId>
    {
        int Version { get; }
    }
}