namespace Arwel.Scripts.Domains.EventBus
{
    public class OdometerConnectionEvent : IEvent
    {
        public bool isConnected;

        public OdometerConnectionEvent(bool isConnected)
        {
            this.isConnected = isConnected;
        }
    }
}