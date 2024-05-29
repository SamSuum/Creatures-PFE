using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour,IInteractable
{
    private string _interactionPrompt = "OPEN";
    [SerializeField] private bool _doorOpen = false;

    public bool locked;

    public string InteractionPrompt => _interactionPrompt;
    [SerializeField] private Animator _anim;
    public bool Interact(Actor player)
    {
        if (!locked)
        {
            if (!_doorOpen)
            {
                Open();
                _interactionPrompt = "CLOSE";
            }
            else
            {
                Close();
                _interactionPrompt = "OPEN";
            }

            _anim.SetBool("Open", _doorOpen);
        }

        else _interactionPrompt = "LOCKED";

        return true;
    }

    private void Open ()
    {
        _doorOpen = true;       
    }

    private void Close()
    {
        _doorOpen = false;
        
    }
}
