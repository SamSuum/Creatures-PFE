
using UnityEngine;
namespace PLAYER
{

    public class Idle : Exploring
    {
        public Idle(Player stateMachine) : base("Idle", stateMachine)
        {
            sm = stateMachine;
            
        }

        public override void Enter()
        {
            base.Enter();
            sm.targetSpeed = 0.0f;
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (sm.input.move != Vector2.zero)
            {
                sm.ChangeState(sm.walkingState);
            }
            sm.stamina.RestoreUnit(10);
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

