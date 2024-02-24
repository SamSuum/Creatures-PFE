using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current
    {
        get; private set;
    }

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

    public event Action<Actor,int,int> onHitTriggerEnter;
    public void HitTriggerEnter(Actor actor, int id,int dmg)
    {
        if (onHitTriggerEnter != null)
        {
            onHitTriggerEnter(actor, id, dmg);
        }
    }
    public event Action<Actor, int> onHitTriggerExit;
    public void HitTriggerExit(Actor actor, int id)
    {
        if (onHitTriggerExit != null)
        {
            onHitTriggerExit(actor, id);
        }
    }



}
