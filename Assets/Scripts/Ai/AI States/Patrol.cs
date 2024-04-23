using UnityEngine;
public class Patrol : IAIBaseState
{
    AIController _ai;
    private Transform currentWaypoint = null;

    WaypointStruct waypoint = new WaypointStruct(0);
    public Patrol(AIController ai)
    {
        _ai = ai;
    }

    public void Enter()
    {
        currentWaypoint = waypoint.FirstWaypoint(_ai.wp);
        waypoint.NavMeshMoveToWaypoint(_ai.agent, currentWaypoint);
    }
    public void Exit() 
    {
        waypoint.NavMeshMoveToWaypoint(_ai.agent, currentWaypoint);
        currentWaypoint = waypoint.NextWaypoint(_ai.wp);
    }

    public void UpdateLogic()
    {
      
        if (waypoint.HasReachedWaypoint(_ai.transform, _ai.dist, currentWaypoint))
        {
            Debug.Log(currentWaypoint+" Reached");
            _ai.ChangeState(_ai.idleState);
        }
        if (_ai.alertStage == AlertStage.Intrigued)
        {
            _ai.ChangeState(_ai.lookOutState);
        }
    }

    public void UpdatePhysic() {}
}