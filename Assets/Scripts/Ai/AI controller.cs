using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
public enum AlertStage
{
    Peaceful,
    Intrigued,
    Alerted
}

public class AIController : AIStateMachine
{
    public Patrol patrolState;
    public Pursuit pursuitState;
    public Search lookOutState;
    public Idle idleState;
    public Flee fleeState;
    public Attack attackState;

    public NavMeshAgent agent;

    [Header("Waypoint")]
    public Transform wp;
    public float dist;

    [Header("FOV")]
    public float fov;
    [Range(0, 360)] public float fovAngle; // in degrees
    public AlertStage alertStage;
    [Range(0, 100)] public float alertLevel; // 0: Peaceful, 100: Alerted

    public Vector3 target;
    public Vector3 dest;
    public Vector3 offset;

    [SerializeField] internal GameObject _weaponL;
    [SerializeField]internal GameObject _weaponR;

    public int health;

    private void Awake()
    {
        patrolState = new Patrol(this);
        pursuitState = new Pursuit(this);
        lookOutState = new Search(this);
        idleState = new Idle(this);
        fleeState = new Flee();
        attackState = new Attack(this);

        agent = GetComponent<NavMeshAgent>();

        alertStage = AlertStage.Peaceful;
        alertLevel = 0;

    }
    public override void OnStart()
    {
        base.OnStart();
        healthBar.gameObject.SetActive(false);

        HP = new GenericHP(health, health);
        stamina = null;
        GameEvents.current.onHitTriggerEnter += OnHitTaken;
        GameEvents.current.onHitTriggerExit += OnHitRecover;
    }
    private void OnDestroy()
    {
        GameEvents.current.onHitTriggerEnter -= OnHitTaken;
        GameEvents.current.onHitTriggerExit -= OnHitRecover;
    }
    
    public override void OnUpdate()
    {
        UpdateDamageCoolDown(this);
        
        if (HasAnimator()) UpdateAnimator(agent.velocity.magnitude, 2);

        SearchForPlayer();

        if(dead) OnDeath(this);
    }
    
    public void SearchForPlayer()
    {
        bool playerInFOV = false;
        Collider[] targetsInFOV = Physics.OverlapSphere(
            transform.position, fov);
        foreach (Collider c in targetsInFOV)
        {
            if (c.CompareTag("Player"))
            {
                float signedAngle = Vector3.Angle(
                    transform.forward,
                    c.transform.position - transform.position);
                if (Mathf.Abs(signedAngle) < fovAngle / 2)
                {
                    playerInFOV = true;
                    target = c.transform.position;
                    dest = target + offset;
                }
                    
                
                break;
            }
        }
        _UpdateAlertState(playerInFOV);
    }    
    private void _UpdateAlertState(bool playerInFOV)
    {
        switch (alertStage)
        {
            case AlertStage.Peaceful:

                if (playerInFOV)
                    alertStage = AlertStage.Intrigued;
                break;

            case AlertStage.Intrigued:

                if (playerInFOV)
                {
                    alertLevel++;
                    if (alertLevel >= 100)
                        alertStage = AlertStage.Alerted;
                }
                else
                {
                    if (alertLevel > 0) alertLevel--;
                    if (alertLevel <= 0)
                    {
                        alertStage = AlertStage.Peaceful;
                    }
                }
                break;

            case AlertStage.Alerted:

                if(!playerInFOV)
                {
                    alertStage = AlertStage.Intrigued;
                }
                break;
        }
    }

    protected override IAIBaseState GetInitState()
    {
        return patrolState;
    }
    
   
}

