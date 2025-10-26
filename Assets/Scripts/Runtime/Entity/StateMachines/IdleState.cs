namespace Entity.StateMachines
{
    public class IdleState : PlayerBaseState
    {
        public IdleState(string name, BaseEntity owner) : base(name, owner)
        {
        }

        public override void EnterState()
        {
            //player.Animator.SetBool(player.IsIdleHash, true);
            // Idle animation is now the default state in the animation tree

        }

        public override void ExitState()
        {
            //player.Animator.SetBool(player.IsIdleHash, false);
            // No cleanup needed since idle is the default state

        }
    }
}
