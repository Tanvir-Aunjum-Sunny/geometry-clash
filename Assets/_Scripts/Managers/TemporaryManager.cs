using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provide utilities for temporary objects (including sound clips, effects, etc)
/// </summary>
public class TemporaryManager : GameSingleton<TemporaryManager>
{
    /// <summary>
    /// Transform for temporary children to use as parent
    /// </summary>
    public Transform TemporaryChildren;
}
