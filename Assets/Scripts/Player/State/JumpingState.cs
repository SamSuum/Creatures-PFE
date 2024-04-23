
using UnityEngine;

namespace PLAYER
{
   
    public class Jumping : InAir
    {
        public Jumping(Player stateMachine) : base("Jumping", stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter() {}
        public override void UpdatePhysics()
        {
            Jump();
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();

            sm.ChangeState(sm.fallingState);            
        }

        public override void Exit()
        {
            ResetJump();
        }
        private void Jump()
        {
            sm.exitingSlope = true;

            sm.jumping = true;

            // the square root of H * -2 * G = how much velocity needed to reach desired height
            sm.verticalVelocity = Mathf.Sqrt(sm.JumpHeight * -2f * Physics.gravity.y);
            sm.rb.AddForce(Vector3.up * sm.verticalVelocity, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            // reset the jump timeout timer
            sm.jumpTimeoutDelta = sm.JumpTimeout;

            sm.exitingSlope = false;

            sm.jumping = false;
        }


    }
   

}

