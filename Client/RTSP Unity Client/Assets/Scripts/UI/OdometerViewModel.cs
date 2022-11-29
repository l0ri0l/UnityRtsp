using System;
using Arwel.Scripts.Domains.EventBus;
using Arwel.Scripts.Odometer;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arwel.Scripts.UI
{
    public class OdometerViewModel : MonoBehaviour, IEventSubscriber<OdometerChangedEvent>,
        IEventSubscriber<OdometerConnectionEvent>
    {
        private bool OdometerStatus { get; set; }
        public Image Lamp;
        public TextMeshProUGUI Status;

        private bool hasConnectionToOdo = false;
        private Action onConnectionChanged;

        [SerializeField] private OdometerWebSocket odoWebSocketBeacon;

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
            //take 5 of integer digits
            newValue %= 100000;
            string newValueString = $"{newValue:F2}";
            
            //removeComma
            newValueString = newValueString.Replace(",", "");

            for (int i = 0; i < odometerDigits.Length; i++)
            {
                try
                {
                    odometerDigits[i].NextSymbol = newValueString[i].ToString();

                }
                catch (Exception ex)
                {
                    odometerDigits[i].NextSymbol = "0";
                }
            }
        }

        void Awake()
        {
            EventBus<OdometerChangedEvent>.Register(this);
            EventBus<OdometerConnectionEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<OdometerChangedEvent>.UnRegister(this);
            EventBus<OdometerConnectionEvent>.UnRegister(this);
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

        public void OnEvent(OdometerConnectionEvent e)
        {
            hasConnectionToOdo = e.isConnected;
            if (hasConnectionToOdo)
            {
                onConnectionChanged = () =>
                {
                    Status.SetText("ODOMETER CONNECTED");
                    Lamp.DOColor(Color.green, 2f).OnComplete(
                        () => { onConnectionChanged = null; }
                    );
                };
            }
            else
            {
                onConnectionChanged = () =>
                {
                    Status.SetText("ODOMETER DISCONNECTED");
                    Lamp.DOColor(Color.red, 2f).OnComplete(
                        () => { onConnectionChanged = null; }
                    );
                };
            }
        }

        private void Update()
        {
            onConnectionChanged?.Invoke();
        }
    }
}
