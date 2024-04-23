using UnityEngine;

namespace PLAYER
{
    public class Sprinting : Exploring
    {
        public Sprinting(Player stateMachine) : base("Sprinting", stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            sm.targetSpeed = sm.SprintSpeed;
            sm.sprinting = true;
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();

            sm.stamina.DecreaseUnit(1);
            sm.staminaBar.SetStamina(sm.stamina.Stamina);

            if (!(sm.input.sprint))
            {
                sm.ChangeState(sm.walkingState);
            }
            if(sm.stamina.Stamina <= 0)
            {
                sm.ChangeState(sm.tiredState);
            }
            if (sm.input.move == Vector2.zero)
            {
                sm.ChangeState(sm.idleState);
            }
        }
        public override void UpdatePhysics()
        {
            base.UpdatePhysics();
        }
        public override void Exit()
        {
            base.Exit();
            sm.sprinting = false;
        }
    }
}

