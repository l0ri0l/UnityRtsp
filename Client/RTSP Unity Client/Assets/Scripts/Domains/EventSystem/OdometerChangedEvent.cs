using Arwel.Scripts.Odometer;

namespace Arwel.Scripts.Domains.EventBus
{
    public class OdometerChangedEvent : IEvent
    {
        public OdometerData odometer;

        public OdometerChangedEvent(OdometerData data)
        {
            odometer = data;
        }

    }
}