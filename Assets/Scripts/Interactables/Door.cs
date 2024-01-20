using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour,IInteractable
{
    private string _interactionPrompt = "OPEN";
    [SerializeField] private GameObject _door;
    [SerializeField] private bool _doorOpen = false;
    public string InteractionPrompt => _interactionPrompt;

    public bool Interact(Player player)
    {
        if(!_doorOpen)
        {
            Open();
            _interactionPrompt = "CLOSE";
        }
        else
        {
            Close();
            _interactionPrompt = "OPEN";
        }
        return true;
    }

    private void Open ()
    {
        _door.SetActive(false);
        _doorOpen = true;
       
    }

    private void Close()
    {
        _door.SetActive(true);
        _doorOpen = false;
        
    }
}
