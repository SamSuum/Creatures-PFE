
using UnityEngine;

namespace PLAYER
{
    public class Exploring : Grounded
    {
        public Exploring(string name, Player stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (sm.input.equip)
            {
                sm.ChangeState(sm.combatState);
                //sm.input.equip = false;
            }

            HandleInteractions();
        }
        public override void UpdatePhysics()
        {
            FreeMovement();

            if (sm.input.jump && sm.jumpTimeoutDelta <= 0.0f && sm.stamina.Stamina > 0)
            {
                Jump();
            }

            base.UpdatePhysics();
        }
        public override void Exit()
        {
            base.Exit();
        }

        private void Jump()
        {
            sm.exitingSlope = true;

            sm.staminaBar.SetStamina(sm.stamina.Stamina);
            sm.stamina.DecreaseUnit(5);

            sm.jumping = true;

            // the square root of H * -2 * G = how much velocity needed to reach desired height
            sm.verticalVelocity = Mathf.Sqrt(sm.JumpHeight * -2f * Physics.gravity.y);
            sm.rb.AddForce(Vector3.up * sm.verticalVelocity, ForceMode.Impulse);
        }

        private void FreeMovement()
        {
            HandleRotation();
            
            sm.direction = GetTargetDirection();
        }
        private Vector3 GetTargetDirection()
        {
            Vector3 inputDirection = new Vector3(sm.input.move.x, 0.0f, sm.input.move.y).normalized;

            sm._targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + sm._mainCamera.transform.eulerAngles.y;

            Vector3 targetDirection = Quaternion.Euler(0.0f, sm._targetRotation, 0.0f) * Vector3.forward;
            return targetDirection;
        }
        private void HandleRotation()
        {

            if (sm.input.move != Vector2.zero)
            {
                float rotation = Mathf.SmoothDampAngle(sm.transform.eulerAngles.y, sm._targetRotation, ref sm._rotationVelocity, sm.RotationSmoothTime);
                sm.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        private void HandleInteractions()
        {
            //Interaction
            sm._numFound = Physics.OverlapSphereNonAlloc(sm.interactionPoint.position, sm.interactionPointRadius, sm._colliders, sm._interactableMask);

            if (sm._numFound > 0)
            {
                //interactions

                sm._interactable = sm._colliders[0].GetComponentInParent<IInteractable>();             

                if (sm._interactable != null)
                {
                    if (!sm._interactionPromptUI.isDisplayed)
                    {
                        sm._interactionPromptUI.SetUp(sm._interactable.InteractionPrompt);
                    }

                    if (sm.input.interact)
                    {
                        sm._interactable.Interact(sm);
                        //sm.input.interact = false;
                    }
                    sm.mimickable = sm._colliders[0].GetComponentInParent<IMimickable>();
                } 
            }
            else
            {
                if (sm._interactable == null) sm._interactable = null;
                if (sm._interactionPromptUI.isDisplayed) sm._interactionPromptUI.Close();
                sm.input.interact = false;
            }
        }


    }

}

