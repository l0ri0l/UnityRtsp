using System;
using Arwel.EventBus;
using UnityEngine;

public class OdometerViewModel : MonoBehaviour, IEventSubscriber<OdometerChangedEvent>
{
    private bool OdometerStatus { get; set; }
    
    [SerializeField]
    private OdometerWebSocket odoWebSocketBeacon;

    private float OdometerValue
    {
        set
        {
            if (Math.Abs(value - _odometerValue) < 0.001) return;
            _odometerValue = value;
            ChangeOdometerWheels(value);
        }
    }
    public OdometerWheelViewModel[] odometerDigits;
    private float _odometerValue;

    private void ChangeOdometerWheels(float newValue)
    {
        var odoInteger = (int) Math.Floor(newValue);
        var odoFractional = newValue - odoInteger;
        
        string integerPartString = odoInteger.ToString();
        
        string fractionalPartString = odoFractional.ToString().Substring(2, 2);
        
        //loops to be sure we will fill all places
        for (var i = 4; i >= 0; i--)
        {
            try
            {
                odometerDigits[i].nextSymbol = integerPartString[i].ToString();
            }
            catch(Exception ex)
            {
                odometerDigits[i].nextSymbol = "0";
            }
        }

        for (var i = 0; i < 2; i++)
        {
            int digitPos = odometerDigits.Length - i - 1;
            try
            {
                odometerDigits[digitPos].nextSymbol = fractionalPartString[i].ToString();
            }
            catch(Exception ex)
            {
                odometerDigits[digitPos].nextSymbol = "0";
            }
        }
    }

    void Awake()
    {
        EventBus<OdometerChangedEvent>.Register(this);
    }
    
    private void OnDestroy()
    {
        EventBus<OdometerChangedEvent>.UnRegister(this);
    }

    public void OnEvent(OdometerChangedEvent e)
    {
        OdometerStatus = e.odometer.OdometerStatus;
        OdometerValue = e.odometer.OdometerValue;
    }

    public void GetOdometerValue()
    {
        var request = new OdometerWebSocketRequest()
        {
            operation = "getCurrentOdometer"
        };

        odoWebSocketBeacon.SendMessage(JsonUtility.ToJson(request));
    }
    
    public void GetRandomStatus()
    {
        var request = new OdometerWebSocketRequest()
        {
            operation = "getRandomStatus"
        };

        odoWebSocketBeacon.SendMessage(JsonUtility.ToJson(request));
    }
}
