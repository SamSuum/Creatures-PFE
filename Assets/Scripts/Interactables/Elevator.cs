using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : WaypointMover, IInteractable
{
    private string _interactionPrompt = "USE";   
    public string InteractionPrompt => _interactionPrompt;
    public bool Interact(Actor player)
    {
        SetNextWaypoint(wp);
        return true;
    }
   
   

}
