using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Resets the transform's position and rotation.
    /// </summary>
    /// <param name="transform">The transform to reset.</param>
    /// <param name="resetPosition">Whether to reset the position.</param>
    /// <param name="resetRotation">Whether to reset the rotation.</param>
    public static void Reset(this Transform transform,
        bool resetPosition = true,
        bool resetRotation = false)
    {
        if (resetPosition) transform.position = Vector3.zero;

        if (resetRotation) transform.rotation = Quaternion.identity;
    }
    
    public static Vector3 Back(this Transform self) 
        => self.position.Multiply(Vector3.back);

    public static Vector3 Left(this Transform self)
        => self.position.Multiply(Vector3.left);

    public static Vector3 Down(this Transform self) 
        => self.position.Multiply(Vector3.down);
}