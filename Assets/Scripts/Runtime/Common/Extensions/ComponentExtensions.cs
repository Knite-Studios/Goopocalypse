using System;
using UnityEngine;

public static class ComponentExtensions
{
    /// <summary>
    /// Checks if the game object has a specific component.
    /// </summary>
    public static bool Has<T>(this Component component) where T : Component
        => component.GetComponent<T>();
    
    public static Component AddComponent(this Component self, Type type) 
        => self.gameObject.AddComponent(type);

    public static T AddComponent<T>(this Component self) where T : Component 
        => self.gameObject.AddComponent<T>();
}