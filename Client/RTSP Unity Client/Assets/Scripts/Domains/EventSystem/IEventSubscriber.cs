namespace Arwel.EventBus
{
    public interface IEventSubscriber<T> where T : class, IEvent
    {
        void OnEvent(T e);
    }
}