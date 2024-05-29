using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Pursuit : IAIBaseState
{
    AIController _ai;
   

    public Pursuit(AIController ai)
    {
        _ai = ai;
    }
    public void Enter() { }
    public void Exit() { }
    public void UpdateLogic() 
    {       
        float distance = (_ai.dest - _ai.agent.transform.position).magnitude;

        Debug.Log(distance);

        if ( _ai.alertStage == AlertStage.Intrigued)
        {
            _ai.ChangeState(_ai.lookOutState);
        }

        if ( distance <= _ai.agent.stoppingDistance + _ai.offset.magnitude)
        {            
            _ai.ChangeState(_ai.attackState);
        }
    }
    public void UpdatePhysic() 
    {
        _ai.agent.destination = _ai.dest;

        Vector3 direction = (_ai.target - _ai.agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        _ai.transform.rotation = Quaternion.Slerp(_ai.transform.rotation, lookRotation, 1.0f * Time.deltaTime);
    }
}