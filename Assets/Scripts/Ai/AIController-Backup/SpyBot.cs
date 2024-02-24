using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Ai
{
    public class AI_SpyBot : MonoBehaviour
    {
        //State Machine
        AI_State currentState;
        public SleepState sleepState = new SleepState();
        public PatrolState patrolState = new PatrolState();
        public GenericHP _spybotHealth = new GenericHP(30, 30);

        public AI_Data data;
        private Hitbox _hitbox;
        private void Awake()
        {
            _hitbox = GetComponentInChildren<Hitbox>();
        }

        private void OnIntruderEscapedOrDead()
        {
            ChangeState(patrolState);
        }

        private void OnIntruderCaught()
        {
            ChangeState(sleepState);
        }

        // Start is called before the first frame update
        void Start()
        {
            GameEvents.current.onAlertTriggerEnter += OnIntruderCaught;
            GameEvents.current.onAlertTriggerExit += OnIntruderEscapedOrDead;

            data.transform = this.transform;
            ChangeState(patrolState);
        }

      

        private void OnDestroy()
        {
            GameEvents.current.onAlertTriggerEnter -= OnIntruderCaught;
            GameEvents.current.onAlertTriggerExit -= OnIntruderEscapedOrDead;
        }

        
        
        void Update()
        {
            if (currentState != null)
            {
                currentState.OnUpdate(data,this);
            }
            //Health and damage
            if (_spybotHealth.Health == 0) Die();
           
        }


        private void Die()
        {
            Destroy(this.gameObject);
        }
        private void TakeDmg(int dmg)
        {
            _spybotHealth.DmgUnit(dmg);
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


