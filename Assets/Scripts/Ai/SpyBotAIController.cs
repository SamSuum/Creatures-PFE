using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Ai
{
    public class AI_SpyBot : MonoBehaviour
    {
        //State Machine
        AI_State currentState;
        public ChaseState chaseState = new ChaseState();
        public PatrolState patrolState = new PatrolState();

        
        public AI_Data data;
       


        // Start is called before the first frame update
        void Start()
        {
            GameEvents.current.onAlertTriggerEnter += OnIntruderCaught;
            data.transform = this.transform;
            ChangeState(patrolState);
        }


        private void OnIntruderCaught()
        {
            //Change state from patrol to pursuit
        }

        private void OnDestroy()
        {
            GameEvents.current.onAlertTriggerEnter -= OnIntruderCaught;
        }

        
        
        void Update()
        {
            if (currentState != null)
            {
                currentState.OnUpdate(data,this);
            }
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


