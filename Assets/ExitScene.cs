using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ExitScene : MonoBehaviour, IInteractable
{
    public UnityEvent Event;
    private string _interactionPrompt = "[E] LEAVE";

   

    public bool locked;

    public string InteractionPrompt => _interactionPrompt;
    public bool Interact(Actor player)
    {
        if (!locked)
        {
            Event.Invoke();
        }
        else _interactionPrompt = "LOCKED";

        return true;
    }

   
}
