using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extended MonoBehaviour utilities
/// </summary>
public class ExtendedMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// Enable waiting for a timeout before performing a delegate function
    /// </summary>
    /// <param name="seconds">Timeout duration</param>
    /// <param name="action">Delegate callback</param>
    public void Wait(float seconds, Action action)
    {
        StartCoroutine(SetTimeout(seconds, action));
    }

    /// <summary>
    /// Perform delegate function after timeout
    /// </summary>
    /// <param name="time">Timeout duration</param>
    /// <param name="callback">Delegate callback</param>
    /// <returns></returns>
    private IEnumerator SetTimeout(float time, Action callback)
    {
        yield return new WaitForSeconds(time);

        callback();
    }
}

