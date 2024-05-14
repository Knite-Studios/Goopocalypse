using System;
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
    public static T GetOrAddComponent<T>(this GameObject self) where T : Component
    {
        if (self.TryGetComponent(out T component))
            return component;

        return self.AddComponent<T>();
    }
    
    /// <summary>
    /// Returns existing component of Type or adds and returns new one.
    /// </summary>
    public static Component GetOrAddComponent(this GameObject self, Type type)
    {
        if (self.TryGetComponent(type, out Component component))
            return component;

        return self.AddComponent(type);
    }
}