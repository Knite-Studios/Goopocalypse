using System;

namespace Managers
{
    public partial class GameManager
    {
        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                OnStateChanged?.Invoke(_state);
            }
        }

        public event Action<GameState> OnStateChanged;
    }
}
