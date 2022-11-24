using Arwel.EventBus;

public class OdometerData
{
    public bool OdometerStatus;
    public float OdometerValue;

    public void UpdateValues(bool status, float value)
    {
        OdometerStatus = status;
        OdometerValue = value;

        var odoEvent = new OdometerChangedEvent(this);
        
        EventBus<OdometerChangedEvent>.Raise(odoEvent);

    }
}
