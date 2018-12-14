using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}


