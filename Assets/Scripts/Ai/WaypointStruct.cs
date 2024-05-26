using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;


public struct WaypointStruct
{
    int _index;
    public WaypointStruct(int index)
    {
        _index = index;
    }

    public bool HasReachedWaypoint(Transform transform, float dist, Transform currentwaypoint)
    {
        if (Vector3.Distance(transform.position, currentwaypoint.position) > dist)
            return false;
        else
            return true;
    }
    public void MoveToWaypoint(Transform t,float speed, Transform currentwaypoint)
    {
        t.position = Vector3.MoveTowards(t.position, currentwaypoint.position, speed * Time.deltaTime);
    }

    public void RigidBodyMoveToWaypoint(Rigidbody rb,float speed,Transform currentwaypoint)
    {        
        Vector3 direction =  currentwaypoint.position - rb.transform.position ;
        rb.MovePosition(rb.position + direction.normalized * speed * Time.deltaTime);
    }
    public void NavMeshMoveToWaypoint(NavMeshAgent agent, Transform currentwaypoint)
    {
        agent.destination = currentwaypoint.position;
    }
    public Transform FirstWaypoint(Transform wp)
    {
        if (wp.transform.childCount > 0 && wp.transform.GetChild(0) != null)
        {
            return wp.transform.GetChild(_index);
        }
        else return null;
    }
    public Transform NextWaypoint(Transform wp)
    {
        if (_index < wp.transform.childCount-1)
        {
            _index++;
            return wp.transform.GetChild(_index);
        }
        else
        {
            _index = 0;
            return wp.transform.GetChild(_index);
        }
        
    }
}