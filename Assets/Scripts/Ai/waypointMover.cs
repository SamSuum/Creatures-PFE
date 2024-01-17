using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointMover : MonoBehaviour
{
    //reference to the waypoint system this will use
    [SerializeField] private Waypoints waypoints;

    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float rotateSpeed = 2.5f;

    [SerializeField] private float distanceThreshhold = 0.1f;

    private Transform currentwaypoint;

    private Quaternion rotationGoal;

    private Vector3 directiontoWaypoint;





    void Start()
    {
        //initial position
        currentwaypoint = waypoints.GetNextWaypoint(currentwaypoint);
        transform.position = currentwaypoint.position;
        //next target position
        currentwaypoint = waypoints.GetNextWaypoint(currentwaypoint);
        transform.LookAt(currentwaypoint);

    }

    
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentwaypoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentwaypoint.position) < distanceThreshhold) 
        {
            currentwaypoint = waypoints.GetNextWaypoint(currentwaypoint) ;
            //transform.LookAt(currentwaypoint);

        }
        rotateTowardsWayPoint();
    }
    private void rotateTowardsWayPoint()
    {
        directiontoWaypoint = (currentwaypoint.position - transform.position).normalized;
        rotationGoal = Quaternion.LookRotation(directiontoWaypoint);
        transform.rotation = Quaternion.Slerp(transform.rotation , rotationGoal , rotateSpeed * Time.deltaTime );

    }



}
