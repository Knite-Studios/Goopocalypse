namespace Entity.StateMachines
{
    public class MovingState : PlayerBaseState
    {
        public MovingState(string name, BaseEntity owner) : base(name, owner)
        {
        }

        public override void EnterState()
        {
            player.Animator.SetBool(player.IsMovingHash, true);
        }

        public override void FixedUpdateState()
        {
            HandleMovement();
        }

        public override void ExitState()
        {
            player.Animator.SetBool(player.IsMovingHash, false);
        }
    }
}
