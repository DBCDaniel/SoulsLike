using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T: Component
{
    private static bool isShuttingDown = false;
    private static object _lock = new object();

    private static T instance;

    public static T Instance
    {
        get
        {
            if (isShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        GameObject singelton = new GameObject();
                        instance = singelton.AddComponent<T>();

                        singelton.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singelton);
                    }
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// Use this for initalization
    /// </summary>
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    protected virtual void OnDestroy()
    {
        isShuttingDown = true;
    }
}
