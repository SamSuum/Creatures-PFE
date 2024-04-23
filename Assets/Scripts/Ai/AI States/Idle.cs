using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Idle : IAIBaseState
{
    AIController _ai;
    float timeOut = 10.0f;
    float timer = 0.0f;
    public Idle(AIController ai)
    {
        _ai = ai;
    }


    public void Enter()
    {
        timer = 0.0f;
        _ai.agent.destination= _ai.transform.position;
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

            if (_ai.alertStage == AlertStage.Alerted)
                _ai.ChangeState(_ai.pursuitState);
        }
        else
        {
            _ai.ChangeState(_ai.patrolState);
        }
    }

    public void UpdatePhysic() { }
}