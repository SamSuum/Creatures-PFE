using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Attack : IAIBaseState
{
    AIController _ai;
    public Attack(AIController ai)
    {
        _ai = ai;
    }

    float _timer = 0;
    public void Enter()
    {
        _ai.combat = true;
        _ai.agent.speed = 0;
    }

    public void Exit()
    {
        _ai.combat = false;
        _ai.agent.speed = 3.5f;
    }

    public void UpdateLogic()
    {
        float distance = (_ai.dest - _ai.agent.transform.position).magnitude;

        _timer += Time.deltaTime;

        if (_timer > 1.0f)
        {
            _ai.attack2 = true;

            //Reset Timer
            _timer = 0;
        }
        else
        {
            _ai.attack2 = false;
        }

        if (distance > 0.1)
        {
            _ai.ChangeState(_ai.pursuitState);
        }
        if (_ai.alertStage == AlertStage.Intrigued)
        {
            _ai.ChangeState(_ai.lookOutState);
        }
    }

    public void UpdatePhysic() 
    {
        Vector3 direction = (_ai.target - _ai.agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        _ai.transform.rotation = Quaternion.Slerp(_ai.transform.rotation, lookRotation, 1.0f * Time.deltaTime);
    }

}