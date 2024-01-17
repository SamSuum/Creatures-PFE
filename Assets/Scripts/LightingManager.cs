using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    private Light _light;
    [SerializeField] private Material _lightMat;
    private void Awake()
    {
        _light = GetComponent<Light>();
       
    }
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onAlertTriggerEnter += ChangeLightsToRed;
        GameEvents.current.onAlertTriggerExit += ChangeLightsToWhite;
    }

    private void ChangeLightsToRed()
    {
        _light.color = Color.red;
        _lightMat.SetColor("_EmissionColor", Color.red);
    }
    private void ChangeLightsToWhite()
    {
        _light.color = Color.white;
        _lightMat.SetColor("_EmissionColor", Color.white);
    }

    private void OnDestroy()
    {
        GameEvents.current.onAlertTriggerEnter -= ChangeLightsToRed;
        GameEvents.current.onAlertTriggerExit -= ChangeLightsToWhite;
    }
}
