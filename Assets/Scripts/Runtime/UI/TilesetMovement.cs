using System.Collections;
using UnityEngine;

namespace Runtime.UI
{
    /// <summary>
    /// Controls the movement of a tileset to simulate camera movement.
    /// </summary>
    public class TilesetMovement : MonoBehaviour
    {
        [Header("Tileset Movement Settings")]
        [Tooltip("Tileset GameObject to move.")]
        [SerializeField] private Transform tilesetTransform;

        [Tooltip("Minimum movement boundaries along the X and Y axis.")]
        [SerializeField] private Vector2 minMovementBounds;

        [Tooltip("Maximum movement boundaries along the X and Y axis.")]
        [SerializeField] private Vector2 maxMovementBounds;

        [Tooltip("The higher the value, the smoother the movement.")]
        [SerializeField] private float damping = 1.0f;

        [Tooltip("Duration to transition between points.")]
        [SerializeField] private float transitionDuration = 2.0f;

        [Tooltip("Minimum distance between two subsequent position points.")]
        [SerializeField] private float minDistanceBetweenPoints = 5.0f;

        private Vector3 _nextPosition;
        private Vector3 _startPosition;
        private float _transitionTime;

        /// <summary>
        /// Initializes the script and starts the movement coroutine.
        /// </summary>
        private void Start()
        {
            if (tilesetTransform == null)
            {
                Debug.LogError("Tileset Transform is not assigned.");
                enabled = false;
                return;
            }

            _startPosition = tilesetTransform.position;
            SetNextPosition(); // Set the first next position
            StartCoroutine(UpdatePosition());
        }

        /// <summary>
        /// Coroutine to update position intermittently rather than every frame.
        /// </summary>
        private IEnumerator UpdatePosition()
        {
            while (true)
            {
                _transitionTime = 0;

                while (_transitionTime < transitionDuration)
                {
                    tilesetTransform.position = Vector3.Lerp(_startPosition, _nextPosition, _transitionTime / transitionDuration);
                    _transitionTime += Time.deltaTime;
                    yield return null;
                }

                _startPosition = _nextPosition;
                SetNextPosition(); // Update to a new position after reaching the current target
            }
        }

        /// <summary>
        /// Sets the next position within the allowed movement bounds ensuring a minimum distance.
        /// </summary>
        private void SetNextPosition()
        {
            do
            {
                float newX = Random.Range(minMovementBounds.x, maxMovementBounds.x);
                float newY = Random.Range(minMovementBounds.y, maxMovementBounds.y);
                _nextPosition = new Vector3(newX, newY, tilesetTransform.position.z);
            }
            while (Vector3.Distance(_startPosition, _nextPosition) < minDistanceBetweenPoints);
        }

        /// <summary>
        /// Resets the target position update on disable.
        /// </summary>
        private void OnDisable()
        {
            StopAllCoroutines(); // Ensure that the coroutine is stopped when the script is disabled
        }
    }
}
