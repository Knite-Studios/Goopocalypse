using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// Ensures only one EventSystem is active in the scene.
    /// Attach to an EventSystem GameObject; disables extras if more than one exists.
    /// </summary>
    public class EventSystemSingleton : MonoBehaviour
    {
        private void Awake()
        {
            var systems = FindObjectsOfType<EventSystem>();
            if (systems.Length <= 1) return;
            for (var i = 1; i < systems.Length; i++)
                systems[i].gameObject.SetActive(false);
        }
    }
}
