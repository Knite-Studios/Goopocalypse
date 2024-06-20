namespace Entity.StateMachines
{
    public class IdleState : PlayerBaseState
    {
        public IdleState(string name, BaseEntity owner) : base(name, owner)
        {
        }

        public override void EnterState()
        {
            player.Animator.SetBool(Idle, true);
        }

        public override void ExitState()
        {
            player.Animator.SetBool(Idle, false);
        }
    }
}
