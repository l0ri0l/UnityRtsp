using System;
using System.Collections;
using Arwel.EventBus;
using UnityEngine;
using System.IO;
using System.Net.WebSockets;
using Task = System.Threading.Tasks.Task;

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

    private bool _isUserDisconnect = false;

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

        BuildAddress(_serverConnectionConfig.address, _serverConnectionConfig.port);

        StartConnection(_connectionURL);
    }

    public async Task StopConnection()
    {
        _isUserDisconnect = true;
        await _wsClient.CloseConnection();
        _isUserDisconnect = false;
    }

    public void StartConnection(string path)
    {
        _wsClient = WebSocketWrapper.Create(path);

        _onConnected += (_) =>
        {
            EventBus<WebSocketVideoConnectionEvent>.Raise(new WebSocketVideoConnectionEvent(true));
            EventBus<ChangeVideoAddress>.Raise(new ChangeVideoAddress(_connectionURL));
        };

        _onDisconnected += async (_) =>
        {
            Debug.Log("Try to reconnect 1");
            EventBus<WebSocketVideoConnectionEvent>.Raise(new WebSocketVideoConnectionEvent(false));
            EventBus<ChangeVideoAddress>.Raise(new ChangeVideoAddress(_connectionURL));
            if (!_isUserDisconnect)
            {
                while (_wsClient.GetStatus() != WebSocketState.Open || _wsClient.GetStatus() != WebSocketState.Connecting)
                {   
                    //try to reconnect every 5 seconds outside main thread
                    await Task.Delay(5000);
                    StartConnection(_connectionURL);    
                }
            }
        };

        _wsClient.OnConnect(_onConnected);
        _wsClient.OnDisconnect(_onDisconnected);

        _wsClient.Connect();
    }

    public static void BuildAddress(string address, string port, string service = "/Echo")
    {
        _connectionURL = $"ws://{address}:{port}{service}";
    }
}
