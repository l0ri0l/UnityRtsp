using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Arwel.Scripts.Domains.EventBus;
using Arwel.Scripts.WebSocket;
using UnityEngine;

namespace Arwel.Scripts.Odometer
{
    public class OdometerWebSocketRequest
    {
        public string operation;
    }

    public class OdometerWebSocket : MonoBehaviour
    {
        private const string ReceiveOdoByRequest = "currentOdometer";
        private const string ReceiveOdoByTime = "odometer_val";
        private const string ReceiveOdoStatusRand = "randomStatus";

        private bool _isDisconnectedByUser;

        private void UpdateByRequest(OdometerOperationResult from, OdometerData to)
        {
            to.UpdateValues(true, from.odometer);
        }

        private void UpdateByTime(OdometerOperationResult from, OdometerData to)
        {
            to.UpdateValues(true, from.value);
        }

        private void UpdateWithRandom(OdometerOperationResult from, OdometerData to)
        {
            bool nextStatus = from.status;
            float nextOdometer = from.odometer;

            Debug.Log($"Status: {from.status}; Value: {from.odometer}");

            if (from.status)
            {
                // Простите, но иногда в теле ответа приходит не то, что вы заявляли в ТЗ. Иногда при status = true не приходит odometer, а иногда при status = false у odometer появляется значение. (см скриншоты из Postman)
                if (from.odometer == 0f)
                {
                    nextStatus = false;
                }

                to.UpdateValues(nextStatus, nextOdometer);
                return;
            }

            nextOdometer = 0f;

            to.UpdateValues(nextStatus, nextOdometer);
        }

        private const string ConnectionUrl = "ws://185.246.65.199:9090/ws";

        private Action<string, WebSocketWrapper> onMessage;
        private Action<WebSocketWrapper> onConnected;
        private Action<WebSocketWrapper> onDisconnected;

        private WebSocketWrapper wsClient;
        private readonly OdometerData _odometerData = new();

        void Awake()
        {
            OdometerUpdater.AddUpdateMethods(ReceiveOdoByRequest, UpdateByRequest);
            OdometerUpdater.AddUpdateMethods(ReceiveOdoByTime, UpdateByTime);
            OdometerUpdater.AddUpdateMethods(ReceiveOdoStatusRand, UpdateWithRandom);

            StartConnection(ConnectionUrl);
        }

        public void SendMessage(string messageToSend)
        {
            wsClient.SendMessage(messageToSend);
        }

        public void StartConnection(string path)
        {
            wsClient = WebSocketWrapper.Create(ConnectionUrl);

            onMessage += (message, _) =>
            {
                var operationResult = JsonUtility.FromJson<OdometerOperationResult>(message);

                OdometerUpdater.UpdateValue(operationResult, _odometerData);
            };

            onConnected += (_) => { EventBus<OdometerConnectionEvent>.Raise(new OdometerConnectionEvent(true)); };

            onDisconnected += async (_) =>
            {
                EventBus<OdometerConnectionEvent>.Raise(new OdometerConnectionEvent(false));
                if (!_isDisconnectedByUser)
                {
                    while (wsClient.GetStatus() != WebSocketState.Open ||
                           wsClient.GetStatus() != WebSocketState.Connecting)
                    {
                        //try to reconnect every 5 seconds outside main thread
                        await Task.Delay(5000);
                        StartConnection(ConnectionUrl);
                    }
                }
            };

            wsClient.OnConnect(onConnected);
            wsClient.OnDisconnect(onDisconnected);
            wsClient.OnMessage(onMessage);

            wsClient.Connect();
        }
    }
}
