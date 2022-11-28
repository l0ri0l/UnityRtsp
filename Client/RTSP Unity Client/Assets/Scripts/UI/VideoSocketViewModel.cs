using System;
using Arwel.Scripts.Domains.EventBus;
using Arwel.Scripts.WebSocket;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arwel.Scripts.UI
{
    public class VideoSocketViewModel : MonoBehaviour, IEventSubscriber<WebSocketVideoConnectionEvent>,
        IEventSubscriber<ChangeVideoAddress>
    {
        private string _address;

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
            if (Address.text != _address)
            {
                Address.SetText(_address);
            }
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
                        () => { onConnectionChanged = null; }
                    );
                };
            }
            else
            {
                onConnectionChanged = () =>
                {
                    Status.SetText("DISCONNECTED");
                    Lamp.DOColor(Color.red, 2f).OnComplete(
                        () => { onConnectionChanged = null; }
                    );
                };
            }
        }

        public void OnEvent(ChangeVideoAddress e)
        {
            var path = e.path;
            _address = (path);
        }
    }
}
