using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Returns true if the game object's layer matches the layer name.
    /// </summary>
    /// <param name="gameObject">The game object to check.</param>
    /// <param name="layerName">The layer name to compare.</param>
    /// <returns>true or false.</returns>
    public static bool CompareLayer(this GameObject gameObject, string layerName)
        => gameObject.layer == LayerMask.NameToLayer(layerName);

    /// <summary>
    /// Returns existing component or adds and returns new one.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject self)
        where T : Component
        => self.TryGetComponent(out T component) ? component : self.AddComponent<T>();

    /// <summary>
    /// Returns existing component of Type or adds and returns new one.
    /// </summary>
    public static Component GetOrAddComponent(this GameObject self, Type type)
        => self.TryGetComponent(type, out var component) ? component : self.AddComponent(type);

    /// <summary>
    /// Checks if the game object has a specific component.
    /// </summary>
    /// <returns>true if the component exists.</returns>
    public static bool Has<T>(this GameObject gameObject)
        where T : Component
        => gameObject.GetComponent<T>();


    /// <summary>
    /// Checks if the colliding object is the player.
    /// </summary>
    public static bool IsPlayer(this Collider2D other)
        => other.gameObject.CompareLayer("Player") || other.gameObject.CompareTag("Player");

    /// <summary>
    /// Checks if the colliding object is the player.
    /// </summary>
    public static bool IsPlayer(this Collision2D other)
        => other.gameObject.CompareLayer("Player") || other.gameObject.CompareTag("Player");

    /// <summary>
    /// Updates the polygon collider shape to match the sprite's shape.
    /// </summary>
    /// <see href="https://discussions.unity.com/t/refreshing-the-polygon-collider-2d-upon-sprite-change/107265/8">
    /// Unity Discussions</see>
    /// <param name="collider">The collider to update.</param>
    /// <param name="sprite">The sprite to use.</param>
    public static void UpdateShapeToSprite(this PolygonCollider2D collider, Sprite sprite)
    {
        // Ensure both valid.
        if (!collider || !sprite) return;
        // Update count.
        collider.pathCount = sprite.GetPhysicsShapeCount();

        // New paths variable.
        var path = new List<Vector2>();

        // Loop path count.
        for (var i = 0; i < collider.pathCount; i++)
        {
            // Clear.
            path.Clear();
            // Get shape.
            sprite.GetPhysicsShape(i, path);
            // Set path.
            collider.SetPath(i, path.ToArray());
        }
    }
}
