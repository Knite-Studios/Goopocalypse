using UnityEngine;

namespace UI
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 100f;

        private void Update()
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }
}
