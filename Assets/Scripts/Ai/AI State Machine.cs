
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class AIStateMachine : Actor
{
    private IAIBaseState _currentState;
    private void Start()
    {
        OnStart();

        _currentState = GetInitState();
        if(_currentState!=null)
        {
            _currentState.Enter();
        }
    }

    private void Update()
    {
        _currentState.UpdateLogic();
        Debug.Log(_currentState);

        OnUpdate();
    }
    private void FixedUpdate()
    {
        _currentState.UpdatePhysic();
    }

    public void ChangeState(IAIBaseState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = newState;
        _currentState.Enter();
    }

    protected virtual IAIBaseState GetInitState()
    {
        return null;
    }

}


