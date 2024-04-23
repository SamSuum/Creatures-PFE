using UnityEngine;

namespace PLAYER
{
    public class Tired: Exploring
    {
        float _timer = 0.0f;
        public Tired(Player stateMachine) : base("Sprinting", stateMachine)
        { 
            sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            sm.targetSpeed = sm.MoveSpeed-sm.MoveSpeed/10;
            //add animation parameter
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();

            sm.staminaBar.SetStamina(sm.stamina.Stamina);

            _timer += Time.deltaTime;

            if(_timer >= 5.0f)
            {
                sm.ChangeState(sm.walkingState);
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
        }
    }
}

