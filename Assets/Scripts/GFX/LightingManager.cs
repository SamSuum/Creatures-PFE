using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    private Light _light;   
    
    private void Awake()
    {
        _light = GetComponent<Light>();       
    }
    
    void Start()
    {
        GameEvents.current.onAlertTriggerEnter += ChangeLightsToRed;
        GameEvents.current.onAlertTriggerExit += ChangeLightsToWhite;
    }

    private void ChangeLightsToRed()
    {
        _light.color = Color.red;       
    }
    private void ChangeLightsToWhite()
    {
        _light.color = Color.white;        
    }

    private void OnDestroy()
    {
        GameEvents.current.onAlertTriggerEnter -= ChangeLightsToRed;
        GameEvents.current.onAlertTriggerExit -= ChangeLightsToWhite;
    }
}
