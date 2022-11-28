using Arwel.EventBus;

public class WebSocketVideoConnectionEvent : IEvent
{
    public bool isConnected;

    public WebSocketVideoConnectionEvent(bool isConnected)
    {
        this.isConnected = isConnected;
    }
}

public class ChangeVideoAddress : IEvent
{
    public string path;

    public ChangeVideoAddress(string path)
    {
        this.path = path;
    }
}
