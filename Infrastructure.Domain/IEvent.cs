namespace Infrastructure.Domain
{
    public interface IEvent<out TSourceId>
    {
        TSourceId SourceId { get; }
    }
}