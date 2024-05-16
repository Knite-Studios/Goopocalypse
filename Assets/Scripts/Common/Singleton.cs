using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A generic Singleton class for creating single instances of a MonoBehaviour.
/// </summary>
/// <typeparam name="T">Type of the Singleton class.</typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // Return the instance if it exists.
            if (_instance != null) return _instance;

            // Find the instance in the scene if it exists.
            _instance = FindObjectOfType<T>();
            if (_instance != null) return _instance;

            // Create a new instance if it doesn't exist.
            var singletonObject = new GameObject();
            _instance = singletonObject.AddComponent<T>();
            singletonObject.name = typeof(T) + " (Singleton)";
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != _instance)
            {
                // If there is already an instance of this Singleton, destroy this one.
                Destroy(gameObject);
            }
        }

        OnAwake();
    }

    /// <summary>
    /// Ensures an instance of the singleton exists.
    /// </summary>
    public static void Initialize()
    {
        if (_instance != null) return;
        _instance = FindObjectOfType<T>();
        if (_instance != null) return;

        var singletonObject = new GameObject();
        _instance = singletonObject.AddComponent<T>();
        singletonObject.name = typeof(T) + " (Singleton)";
    }

    protected virtual void OnAwake() { }

    protected virtual void OnEnable()
    {
        // Subscribes to the scene loaded event.
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Subscribes to the scene unloaded event.
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribes from the scene loaded event.
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // Unsubscribes from the scene unloaded event.
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    protected virtual void OnSceneUnloaded(Scene scene) { }
}


/// <summary>
/// A generic NetworkSingleton class for creating single instances of a NetworkBehaviour.
/// </summary>
/// <typeparam name="T">Type of the NetworkSingleton class.</typeparam>
[RequireComponent(typeof(NetworkIdentity))]
public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // Return the instance if it exists.
            if (_instance != null) return _instance;

            // Find the instance in the scene if it exists.
            _instance = FindObjectOfType<T>();
            if (_instance != null) return _instance;

            // Create a new instance if it doesn't exist.
            var NetworkSingletonObject = new GameObject();
            _instance = NetworkSingletonObject.AddComponent<T>();
            NetworkSingletonObject.name = typeof(T) + " (NetworkSingleton)";
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != _instance)
            {
                // If there is already an instance of this NetworkSingleton, destroy this one.
                Destroy(gameObject);
            }
        }

        OnAwake();
    }

    /// <summary>
    /// Ensures an instance of the NetworkSingleton exists.
    /// </summary>
    public static void Initialize()
    {
        if (_instance != null) return;
        _instance = FindObjectOfType<T>();
        if (_instance != null) return;

        var NetworkSingletonObject = new GameObject();
        _instance = NetworkSingletonObject.AddComponent<T>();
        NetworkSingletonObject.name = typeof(T) + " (NetworkSingleton)";
    }

    protected virtual void OnAwake() { }

    protected virtual void OnEnable()
    {
        // Subscribes to the scene loaded event.
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Subscribes to the scene unloaded event.
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribes from the scene loaded event.
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // Unsubscribes from the scene unloaded event.
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    protected virtual void OnSceneUnloaded(Scene scene) { }
}
