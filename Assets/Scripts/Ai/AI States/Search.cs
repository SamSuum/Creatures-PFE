using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class Search : IAIBaseState
{
    AIController _ai;
    float timeOut = 10.0f;
    float timer = 0.0f;
    public Search(AIController ai)
    {
        _ai = ai;
    }

    
    public void Enter()
    {
        timer = 0.0f;
        _ai.agent.destination = _ai.dest;
    }

    public void Exit()
    {
        timer = 0.0f;
    }

    public void UpdateLogic()
    {
        if (timer < timeOut)
        {            
            timer += Time.deltaTime;

            if(_ai.alertStage == AlertStage.Alerted) 
                _ai.ChangeState(_ai.pursuitState);
        }
        else
        {
            _ai.ChangeState(_ai.patrolState);
        }
    }
   
    public void UpdatePhysic() 
    {
        Vector3 direction = (_ai.target - _ai.agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        _ai.transform.rotation = Quaternion.Slerp(_ai.transform.rotation, lookRotation, 1f * Time.deltaTime);

    }
}