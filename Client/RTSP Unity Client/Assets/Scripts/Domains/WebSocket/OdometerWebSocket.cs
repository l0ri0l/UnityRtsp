using System;
using System.Threading.Tasks;
using RtspTest.Domains.Odometer;
using UnityEngine;

public class OdometerWebSocketRequest
{
    public string operation;
}

public class OdometerWebSocket : MonoBehaviour
{
    private const string ReceiveOdoByRequest = "currentOdometer";
    private const string ReceiveOdoByTime = "odometer_val";
    private const string ReceiveOdoStatusRand = "randomStatus";

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
        
        if (from.status)
        {
            // Простите, но иногда в теле ответа приходит не то, что вы заявляли в ТЗ. Иногда при status = true не приходит odometer, а иногда при status = false у odometer появляется значение. (см скриншоты из Postman)
            if (from.odometer != 0f)
            {
                nextOdometer = from.odometer;
            }

            to.UpdateValues(nextStatus, nextOdometer);
            return;
        }

        nextOdometer = 0f;
        
        to.UpdateValues(nextStatus, nextOdometer);
    }
    
    private const string connection = "ws://185.246.65.199:9090/ws";

    private Action<string, WebSocketWrapper> onMessage;
    private WebSocketWrapper wsClient;
    private readonly OdometerData _odometerData = new();

    void Awake()
    {
        OdometerUpdater.AddUpdateMethods(ReceiveOdoByRequest, UpdateByRequest);
        OdometerUpdater.AddUpdateMethods(ReceiveOdoByTime, UpdateByTime);
        OdometerUpdater.AddUpdateMethods(ReceiveOdoStatusRand, UpdateWithRandom);
        
        wsClient = WebSocketWrapper.Create(connection);

        onMessage += (message, wrapper) =>
        {
            Debug.Log("Message!");
            var operationResult = JsonUtility.FromJson<OdometerOperationResult>(message);
            
            OdometerUpdater.UpdateValue(operationResult, _odometerData);
        };

        wsClient.OnMessage(onMessage);

        wsClient.Connect();
    }

    public async Task OnMessage(string message)
    {
        Debug.Log("Message!");
        var operationResult = JsonUtility.FromJson<OdometerOperationResult>(message);
            
        OdometerUpdater.UpdateValue(operationResult, _odometerData);
        await Task.CompletedTask;
    }
    
    public void SendMessage(string messageToSend)
    { 
        wsClient.SendMessage(messageToSend);
    }
}
