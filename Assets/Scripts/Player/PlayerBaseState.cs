

namespace PLAYER
{
    public abstract class PlayerBaseState
    {
        public string name;
        protected PlayerStateMachine _stateMachine;
        protected Player sm;
        public PlayerBaseState(string name, PlayerStateMachine stateMachine)
        {
            this.name = name;
            this._stateMachine = stateMachine;
        }
        public virtual void Enter() { }
        public virtual void UpdateLogic() { }
        public virtual void UpdatePhysics() { }
        public virtual void Exit() { }

    }
}

