using System;
using Arwel.EventBus;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSocketViewModel : MonoBehaviour, IEventSubscriber<WebSocketVideoConnectionEvent>, IEventSubscriber<ChangeVideoAddress>
{
    public WebSocketVideo webSocket;
    public Image Lamp;
    public TextMeshProUGUI Address;
    public TextMeshProUGUI Status;

    private bool isConnected;
    private Action onConnectionChanged;
    
    void Awake()
    {
        EventBus<WebSocketVideoConnectionEvent>.Register(this);
        EventBus<ChangeVideoAddress>.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        onConnectionChanged?.Invoke();
    }

    private void OnDestroy()
    {
        EventBus<WebSocketVideoConnectionEvent>.UnRegister(this);
        EventBus<ChangeVideoAddress>.UnRegister(this);
    }

    public void OnEvent(WebSocketVideoConnectionEvent e)
    {
        isConnected = e.isConnected;
        if (isConnected)
        {
            onConnectionChanged = () =>
            {
                Status.SetText("CONNECTED");
                Lamp.DOColor(Color.green, 2f).OnComplete(
                    () =>
                    {
                        onConnectionChanged = null;
                    }
                );
            };
        }
        else
        {
            onConnectionChanged = () =>
            {
                Status.SetText("DISCONNECTED");
                Lamp.DOColor(Color.red, 2f).OnComplete(
                    () =>
                    {
                        onConnectionChanged = null;
                    }
                );
            };
        }
    }

    public void OnEvent(ChangeVideoAddress e)
    {
        var path = e.path;
        Address.SetText(path);

        webSocket.StopConnection();
        webSocket.StartConnection(path);
    }
}
