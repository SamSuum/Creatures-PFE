
using UnityEngine;

namespace PLAYER
{
    
    public class Falling : InAir
    {

        public Falling(Player stateMachine) : base("Falling", stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter()
        {
            sm.falling = true;
            ResetJump();
        }
        public override void UpdatePhysics()
        {

        }
        public override void UpdateLogic()
        {
            FallTimeout();
            base.UpdateLogic();
            
        }

        public override void Exit()
        {
            // reset the fall timeout timer
            sm.fallTimeoutDelta = sm.FallTimeout;

            sm.falling = false;
        }
        
        private void FallTimeout()
        {
            // fall timeout
            if (sm.fallTimeoutDelta >= 0.0f)
            {
                sm.fallTimeoutDelta -= Time.deltaTime;
            }
        }

        private void ResetJump()
        {
            // reset the jump timeout timer
            sm.jumpTimeoutDelta = sm.JumpTimeout;

            sm.exitingSlope = false;

            sm.jumping = false;

            sm.input.jump = false;
        }

    }

   
}

