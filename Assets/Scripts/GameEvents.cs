using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action onAlertTriggerEnter;
       
    public void AlertTriggerEnter()
    {
        if(onAlertTriggerEnter!=null)
        {
            onAlertTriggerEnter();
        }
    }


    public event Action onAlertTriggerExit;

    public void AlertTriggerExit()
    {
        if (onAlertTriggerExit != null)
        {
            onAlertTriggerExit();
        }
    }
}
