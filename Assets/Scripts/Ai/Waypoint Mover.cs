using Ai;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class WaypointMover : MonoBehaviour
{
    private Transform currentwaypoint;
    [SerializeField] protected Waypoints wp;
    [SerializeField] float speed ;
    [SerializeField] float dist ;
    public  void Start()
    {
        InitWaypoints();
    }

    public void Update()
    {
        MoveToWaypoint();
    }
    
    private void InitWaypoints()
    {
        //initial position
        currentwaypoint = wp.GetNextWaypoint(currentwaypoint);
        //next target position
        transform.position = currentwaypoint.position;
    }

    protected void MoveToWaypoint()
    {
        if (Vector3.Distance(transform.position, currentwaypoint.position) > dist)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentwaypoint.position, speed * Time.deltaTime);
        }
    }

    protected void SetNextWaypoint(Waypoints wp)
    {        
            currentwaypoint = wp.GetNextWaypoint(currentwaypoint);
    }

}


