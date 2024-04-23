

using UnityEngine;

namespace PLAYER
{
    public class Walking : Exploring
    {
        public Walking(Player stateMachine) : base("Walking", stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            sm.targetSpeed = sm.MoveSpeed;
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (sm.input.sprint && sm.stamina.Stamina > 0)
            {
                sm.ChangeState(sm.sprintingState);
            }
            if (sm.input.move == Vector2.zero)
            {
                sm.ChangeState(sm.idleState);
            }

            sm.stamina.RestoreUnit(5);
            sm.staminaBar.SetStamina(sm.stamina.Stamina);
        }
        public override void UpdatePhysics()
        {
            base.UpdatePhysics();
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}

