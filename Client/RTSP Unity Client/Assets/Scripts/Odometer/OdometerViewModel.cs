using System;
using System.Collections;
using System.Collections.Generic;
using Arwel.EventBus;
using RtspTest.Domains.Odometer;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OdometerViewModel : MonoBehaviour, IEventSubscriber<OdometerChangedEvent>
{
    private const string ReceiveOdoByRequest = "currentOdometer";
    private const string ReceiveOdoByTime = "odometer_val";
    private const string ReceiveOdoStatusRand = "randomStatus";

    private void UpdateByRequest(OdometerOperationResult from, OdometerData to)
    {
        to.OdometerStatus = true;
        to.OdometerValue = from.odometer;
    }

    private void UpdateByTime(OdometerOperationResult from, OdometerData to)
    {
        to.OdometerStatus = true;
        to.OdometerValue = from.value;
    }

    private void UpdateWithRandom(OdometerOperationResult from, OdometerData to)
    {
        Debug.Log(from.status);
                
        to.OdometerStatus = from.status;
        if (from.status)
        {
            // Простите, но иногда в теле ответа приходит не то, что вы заявляли в ТЗ. Иногда при status = true не приходит odometer, а иногда при status = false у odometer появляется значение. (см скриншоты из Postman)
            if (from.odometer != 0f)
            {
                to.OdometerValue = from.odometer;
            }

            return;
        }

        to.OdometerValue = 0f;
    }
    
    public bool OdometerStatus { get; set; }
    public float OdometerValue { get; set; }

    public Text odometerTitle;
    public Text odometerValue;
        
    
    void Awake()
    {
        OdometerUpdater.AddUpdateMethods(ReceiveOdoByRequest, UpdateByRequest);
        OdometerUpdater.AddUpdateMethods(ReceiveOdoByTime, UpdateByTime);
        OdometerUpdater.AddUpdateMethods(ReceiveOdoStatusRand, UpdateWithRandom);
        
        EventBus<OdometerChangedEvent>.Register(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        odometerTitle.text = "ODOMETER: ";
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
