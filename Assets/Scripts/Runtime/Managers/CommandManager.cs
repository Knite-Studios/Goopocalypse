using System.Collections.Generic;
using Commands;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class CommandManager : MonoSingleton<CommandManager>
    {
        private readonly Queue<ICommand> _commands = new();

        private void Update()
        {
            if (_commands.Count > 0)
                _commands.Dequeue().Execute();
        }

        protected override void OnSceneUnloaded(Scene scene)
            => _commands.Clear();

        public void AddCommand(ICommand command)
            => _commands.Enqueue(command);
    }
}
