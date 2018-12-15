using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{
    /// <summary>
    /// Indicate whether a layermask contains a layer
    /// </summary>
    /// <param name="mask">Layermask</param>
    /// <param name="layer">Layer being checked</param>
    /// <returns>Whether layermask contains layer</returns>
    public static bool LayerContains(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}
