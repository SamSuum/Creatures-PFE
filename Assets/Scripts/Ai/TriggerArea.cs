using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameEvents.current.AlertTriggerEnter();
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            GameEvents.current.AlertTriggerExit();
        }
    }
}
