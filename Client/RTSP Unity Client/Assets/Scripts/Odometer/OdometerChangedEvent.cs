using Arwel.EventBus;

public class OdometerChangedEvent : IEvent
{
    public OdometerData odometer;

    public OdometerChangedEvent(OdometerData data)
    {
        odometer = data;
    }
    
}
