using System;
using UnityEngine;

public static class ComponentExtensions
{
    /// <summary>
    /// Checks if the game object has a specific component.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool Has<T>(this Component gameObject) where T : Component
        => gameObject.GetComponent<T>();
    
    public static Component AddComponent(this Component self, Type type) 
        => self.gameObject.AddComponent(type);

    public static T AddComponent<T>(this Component self) where T : Component 
        => self.gameObject.AddComponent<T>();
}