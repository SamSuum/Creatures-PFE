using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IInteractable
{
    private string _interactionPrompt = "USE";   
    public string InteractionPrompt => _interactionPrompt;
    private WaypointStruct ws = new WaypointStruct(0);

    private Transform _currentWaypoint = null;
    [SerializeField] private Transform _wp;
    [SerializeField] float _speed;
    [SerializeField] float _dist;
    public void Start()
    {
        _currentWaypoint = ws.FirstWaypoint(_wp);
    }

    public void Update()
    {
        if(!ws.HasReachedWaypoint(this.transform,_dist,_currentWaypoint))
           ws.MoveToWaypoint(this.transform, _speed, _currentWaypoint);
    }

    public bool Interact(Actor player)
    {
        if (ws.HasReachedWaypoint(this.transform, _dist, _currentWaypoint))
            _currentWaypoint = ws.NextWaypoint(_wp);
        return true;
    }


}
