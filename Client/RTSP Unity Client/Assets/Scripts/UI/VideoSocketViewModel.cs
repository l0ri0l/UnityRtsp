using System;
using System.Threading.Tasks;
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
                    Address.SetText($"ws://{VideoPath.Server}:{VideoPath.Port}/Echo");
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
                    Address.SetText($"ws://{VideoPath.Server}:{VideoPath.Port}/Echo");
                    Lamp.DOColor(Color.red, 2f).OnComplete(
                        () => { onConnectionChanged = null; }
                    );
                };
            }
        }

        public async void OnEvent(ChangeVideoAddress e)
        {
            var path = $"ws://{e.Server}:{e.Port}/Echo";
            _address = (path);
            Debug.Log("building");
            await UpdateConnection(e.Server, e.Port);
        }

        private async Task UpdateConnection(string server, string port)
        {
            await webSocket.StopConnection();
            WebSocketVideo.BuildAddress(server, port);
            webSocket.StartConnection();
        }
    }
}
