
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public interface IAIBaseState
{
    public void Enter();
    public void UpdateLogic();
    public void UpdatePhysic();
    public void Exit();
}


