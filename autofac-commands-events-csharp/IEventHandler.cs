namespace Infrastructure
{
    public interface IEventHandler<in T> where T : IEvent
    {
        void Handle(T domainEvent);
    }
}