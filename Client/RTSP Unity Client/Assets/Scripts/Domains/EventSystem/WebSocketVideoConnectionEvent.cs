namespace Arwel.Scripts.Domains.EventBus
{
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
        public string Server;
        public string Port;
        public string Video;

        public ChangeVideoAddress(string server, string port, string video)
        {
            Server = server;
            Port = port;
            Video = video;
        }
    }
}