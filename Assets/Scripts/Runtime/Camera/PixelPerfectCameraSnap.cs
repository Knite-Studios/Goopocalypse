using UnityEngine;

/// <summary>
/// Snaps the camera position to a pixel grid in LateUpdate (after Cinemachine).
/// Eliminates sub-pixel camera movement that causes jitter in pixel art games.
/// Attach to the Main Camera (the one driven by Cinemachine Brain).
/// </summary>
[DefaultExecutionOrder(500)]
public class PixelPerfectCameraSnap : MonoBehaviour
{
    [Tooltip("Pixels per unit of your sprites (e.g. 16 or 32). Camera position snaps to 1/this in world space.")]
    [SerializeField] private int pixelsPerUnit = 16;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam != null && !_cam.orthographic)
            Debug.LogWarning("[PixelPerfectCameraSnap] Camera is not orthographic; snapping may still help but is typically for 2D.");
    }

    private void LateUpdate()
    {
        if (_cam == null) return;

        float pixelSize = 1f / pixelsPerUnit;
        Vector3 pos = transform.position;
        transform.position = new Vector3(
            Mathf.Round(pos.x / pixelSize) * pixelSize,
            Mathf.Round(pos.y / pixelSize) * pixelSize,
            pos.z
        );
    }
}
