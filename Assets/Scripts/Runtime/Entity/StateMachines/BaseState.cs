namespace Entity.StateMachines
{
    public abstract class BaseState<T>
    {
        public string Name;
        protected T Owner;

        protected BaseState(string name, T owner)
        {
            Name = name;
            Owner = owner;
        }

        public virtual void EnterState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
        public virtual void ExitState() { }
    }
}
