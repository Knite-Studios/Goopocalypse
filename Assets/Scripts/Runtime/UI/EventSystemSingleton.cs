using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// Ensures only one EventSystem is active in the scene.
    /// Attach to an EventSystem GameObject; disables this one if another is already active.
    /// Prevents "There can be only one active Event System" errors when multiple scenes or prefabs add one.
    /// </summary>
    public class EventSystemSingleton : MonoBehaviour
    {
        private void Awake()
        {
            var systems = FindObjectsOfType<EventSystem>();
            for (var i = 1; i < systems.Length; i++)
                systems[i].enabled = false;
        }
    }
}
