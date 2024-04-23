

namespace PLAYER
{
   
    public class InAir : PlayerBaseState
    {

        public InAir (string name, Player stateMachine) : base(name, stateMachine)
        {
            sm = stateMachine;
        }

        public override void Enter() {}
        public override void UpdatePhysics() {}
        public override void UpdateLogic()
        {
            if (sm.grounded)
            {
                sm.ChangeState(sm.idleState);
            }

        }

        public override void Exit() {}
    }
   
}

