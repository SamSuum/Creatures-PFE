
using Cinemachine.Utility;
using UnityEngine;

namespace PLAYER
{
    public class Grounded : PlayerBaseState
    {

        public Grounded(string name, Player stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;
        }
        
        public override void UpdatePhysics()
        {

            if (!sm.input.jump)
                SnapToGround();
            

            if (OnSlope() && !sm.exitingSlope)
            {
                MoveOnSlope();
            }
            
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (!sm.grounded) sm.ChangeState(sm.fallingState);

            HandleHorizontalVelocity();
            handleAnimationBlend();
            HandleVerticalVelocity();
        }

      
        public void HandleVerticalVelocity()
        {
            
            // stop our velocity dropping infinitely when grounded
            if (sm.verticalVelocity < 0.0f)
            {
                sm.verticalVelocity = -2f;
            }

            if (sm.verticalVelocity < sm.terminalVelocity)
            {
                sm.verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }

            // jump timeout
            if (sm.jumpTimeoutDelta >= 0.0f)
            {
                sm.jumpTimeoutDelta -= Time.deltaTime;
            }

        }
        private void handleAnimationBlend()
        {
            sm.motionSpeed = (sm.Inputmagnitude == 0) ? 10 : sm.Inputmagnitude;
            sm.animationBlend = Mathf.Lerp(sm.animationBlend, sm.targetSpeed, Time.deltaTime * sm.motionSpeed);
            if (sm.animationBlend < 0.01f) sm.animationBlend = 0f;
        }

        
        private void HandleHorizontalVelocity()
        {
            float currentHorizontalSpeed = new Vector3(sm.rb.velocity.x, 0.0f, sm.rb.velocity.z).magnitude;
            float speedOffset = 0.1f;
            sm.Inputmagnitude = sm.input.analogMovement ? sm.input.move.magnitude : 1f;
            if (currentHorizontalSpeed < sm.targetSpeed - speedOffset || currentHorizontalSpeed > sm.targetSpeed + speedOffset)
            {
                sm.speed = Mathf.Lerp(currentHorizontalSpeed, sm.targetSpeed * sm.Inputmagnitude, Time.deltaTime * sm.SpeedChangeRate);
                // round speed to 3 decimal places
                sm.speed = Mathf.Round(sm.speed * 1000f) / 1000f;
            }
            else sm.speed = sm.targetSpeed;           
            
        }
 

        private bool OnSlope()
        {            
            float angle = Vector3.Angle(Vector3.up, sm.groundHit.normal);
            return angle < sm.maxSlopeAngle && angle != 0;
        }

        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(sm.direction, sm.groundHit.normal);
        }

        private void MoveOnSlope()
        {
            sm.direction = GetSlopeMoveDirection();
        }

        private void SnapToGround()
        {
            Vector3 dir = sm.groundHit.normal;
            sm.rb.AddForce( dir * -30f, ForceMode.Force);
        }


    }
}

