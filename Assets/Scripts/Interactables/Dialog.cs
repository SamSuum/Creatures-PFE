using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dialog : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt = "Talk to";
    public string InteractionPrompt => _prompt;

    public bool inDialog = false;

    public UnityEvent OnEnter;

    public bool Interact(Actor player)
    {
        OnEnter.Invoke();
        return true;
    }

   
}
