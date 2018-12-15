using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Taken From: https://gist.github.com/michidk/640765fc570220333ac1
// Possible Ideas: https://gist.github.com/mstevenson/4325117

/// <summary>
/// Enable singleton pattern for one-off classes
/// </summary>
/// <typeparam name="T">Type of Instance provided by singleton</typeparam>
public class GameSingleton<T> : ExtendedMonoBehaviour {

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) + " is needed in the scene but not found");
                }
            }

            return instance;
        }
    }*/
}


