
using UnityEngine;

namespace PLAYER
{
    public class Combat : Grounded
    {
        public Combat(Player stateMachine) : base("Combat", stateMachine)
        {
            sm = stateMachine;
        }
        public override void Enter()
        {
            base.Enter();
            sm.combat = true;
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (sm.input.equip)
            {
                sm.ChangeState(sm.idleState);
                //sm.input.equip = false;
            }

            sm.timeSinceAttack += Time.deltaTime;

            HeavyAttack();
            QuickAttack();
            Block();

            if (sm.input.move != Vector2.zero) sm.targetSpeed = sm.MoveSpeed;
            else sm.targetSpeed = 0;

        }
        public override void UpdatePhysics()
        {
            base.UpdatePhysics();
            MoveRelativeToCamera();
        }
        public override void Exit()
        {
            base.Exit();
            sm.combat = false;
        }


        private void Block()
        {
            if (sm.input.block)
            {
                Debug.Log("Blocking");

                sm.blocking = true;
                sm._weaponR.tag = "Untagged";
                sm._weaponL.tag = "Untagged";
            }
            else
            {
                sm.blocking = false;
            }
        }
        private void HeavyAttack()
        {
            if (sm.input.heavyAttack && sm.timeSinceAttack > .8f)
            {
                Debug.Log("Strong Attack");

                sm.attack2 = true;

                sm.input.heavyAttack = false;
                sm._weaponR.tag = "Dmg";
                sm._weaponL.tag = "Dmg";
                //Reset Timer
                sm.timeSinceAttack = 0;
            }
            else
            {
                sm.attack2 = false;

                if (sm.timeSinceAttack > .8f)
                {
                    sm._weaponR.tag = "Untagged";
                    sm._weaponL.tag = "Untagged";
                }

            }
        }
        private void QuickAttack()
        {

            if (sm.input.quickAttack && sm.timeSinceAttack > .8f)
            {
                sm.attack1 = true;
                sm.currentAttack++;
                //sm.input.quickAttack = false;
                sm._weaponR.tag = "Dmg";
                sm._weaponL.tag = "Dmg";

                Debug.Log("Quick Attack");

                //Reset Timer
                sm.timeSinceAttack = 0;
            }
            else
            {
                sm.attack1 = false;

                if (sm.timeSinceAttack > .8f)
                {
                    sm._weaponR.tag = "Untagged";
                    sm._weaponL.tag = "Untagged";
                }

            }
        }
        private void MoveRelativeToCamera()
        {
            float Verticalinput = sm.input.move.y;
            float HorizontalInput = sm.input.move.x;

            Vector3 forward = sm._mainCamera.transform.forward;
            Vector3 right = sm._mainCamera.transform.right;
            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;

            //move direction
            sm.direction = Verticalinput * forward + HorizontalInput * right;

            //Handle player rotation
            sm._targetRotation = sm._mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(sm.transform.eulerAngles.y, sm._targetRotation, ref sm._rotationVelocity, sm.RotationSmoothTime);
            sm.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            sm._animator.SetFloat("VelocityZ", Verticalinput);
            sm._animator.SetFloat("VelocityX", HorizontalInput);

        }
    }
}

