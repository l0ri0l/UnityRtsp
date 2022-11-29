using System;
using UnityEngine;
using System.IO;
using System.Net.WebSockets;
using Arwel.Scripts.Domains.EventBus;
using Arwel.Scripts.UI;
using Task = System.Threading.Tasks.Task;

namespace Arwel.Scripts.WebSocket
{
    public class WebSocketVideoConfig
    {
        public string address;
        public string port;

        public WebSocketVideoConfig(string address, string port)
        {
            this.address = address;
            this.port = port;
        }
    }

    public class WebSocketVideo : MonoBehaviour
    {
        private const string ConfigFilePath = "Assets/Resources/config.txt";

        private static string _connectionURL = "";

        [NonSerialized] public bool IsUserDisconnect = false;

        private WebSocketVideoConfig _serverConnectionConfig;

        private WebSocketWrapper _wsClient;

        private Action<WebSocketWrapper> _onConnected;
        private Action<WebSocketWrapper> _onDisconnected;

        private void Awake()
        {
            StreamReader reader = new StreamReader(ConfigFilePath);
            var jsonConfig = reader.ReadToEnd();
            reader.Close();

            _serverConnectionConfig = JsonUtility.FromJson<WebSocketVideoConfig>(jsonConfig);

            VideoPath.Server = _serverConnectionConfig.address;
            VideoPath.Port = _serverConnectionConfig.port;
        }

        void Start()
        {
            EventBus<ChangeVideoAddress>.Raise(new ChangeVideoAddress(VideoPath.Server, VideoPath.Port, ""));
        }

        public async Task StopConnection()
        {
            if (_wsClient != null)
            {
                await _wsClient.CloseConnection();
            }
        }

        public void StartConnection()
        {
            Debug.Log("Try connect");
            _wsClient = WebSocketWrapper.Create(_connectionURL);

            _onConnected += (_) =>
            {
                EventBus<WebSocketVideoConnectionEvent>.Raise(new WebSocketVideoConnectionEvent(true));
            };

            async void OnDisconnected(WebSocketWrapper _)
            {
                Debug.Log(_connectionURL);
                EventBus<WebSocketVideoConnectionEvent>.Raise(new WebSocketVideoConnectionEvent(false));
                
                if (IsUserDisconnect)
                {
                    IsUserDisconnect = false;
                    return;
                }
                
                while (_wsClient.GetStatus() != WebSocketState.Open || _wsClient.GetStatus() != WebSocketState.Connecting)
                {
                    Debug.Log(_wsClient.GetStatus());
                    //try to reconnect every 5 seconds outside main thread
                    await Task.Delay(5000);
                    StartConnection();
                }
            }

            _onDisconnected += OnDisconnected;

            _wsClient.OnConnect(_onConnected);
            _wsClient.OnDisconnect(_onDisconnected);

            _wsClient.Connect();
        }

        public static void BuildAddress(string address, string port)
        {
            Debug.Log("builded");
            _connectionURL = $"ws://{address}:{port}/Echo";
        }
    }
}
