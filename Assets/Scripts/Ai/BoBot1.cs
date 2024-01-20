using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Ai
{
    public class AI_BoBot : MonoBehaviour
    {
        //State Machine
        AI_State currentState;
        public SleepState sleepState = new SleepState();
        public PatrolState patrolState = new PatrolState();
        public GenericHP _botHealth = new GenericHP(30, 30);        
        public AI_Data data;

        private Hitbox _hitbox;
        private void Awake()
        {
            _hitbox = GetComponentInChildren<Hitbox>();
        }

        // Start is called before the first frame update
        void Start()
        {
           data.transform = this.transform;
            ChangeState(patrolState);
        }


        void Update()
        {
            if (currentState != null)
            {
                currentState.OnUpdate(data,this);
            }
            //Health and damage
            if (_botHealth.Health == 0) Die();
            if (_hitbox.hit)
            {
                TakeDmg(_hitbox.dmg);
            }
        }
        private void TakeDmg(int dmg)
        {
            _botHealth.DmgUnit(dmg);
        }

        private void Die()
        {
            Destroy(this.gameObject);
        }


        public void ChangeState(AI_State newState)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }
            currentState = newState;
            currentState.OnEnter(data);
        }

    }
}


