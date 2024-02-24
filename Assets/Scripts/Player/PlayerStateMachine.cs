using Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// State machine + Even Listener + Camera + groundcheck
/// </summary>
public class PlayerStateMachine: Actor
{
    //Finite State Machine
    private PlayerBaseState currentState;
  
    public virtual void OnStart() {}
    public virtual void OnFixedUpdate() {}
    public virtual void OnUpdate() {}

    private void Start()
    {
        currentState = GetInitState();

        if(currentState !=null)
        {
            currentState.Enter();
        }

        OnStart();
    }
   

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateLogic();
        }
        OnUpdate();
    }
    private void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.UpdatePhysics();
        }

        OnFixedUpdate();

    }
   
   
    public void ChangeState(PlayerBaseState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    protected virtual PlayerBaseState GetInitState()
    {
        return null;
    }

    //Debug
    private void OnGUI()
    {
        string content = currentState != null ? currentState.name : "no current state";
       
        GUILayout.Label($"<color='black><size=40>{content}</size></color>");
    }
}