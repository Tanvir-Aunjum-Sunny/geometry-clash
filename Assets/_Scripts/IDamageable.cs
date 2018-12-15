using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Receive a hit from something
    /// </summary>
    /// <param name="damage">Damage received</param>
    /// <param name="collider">Colliding entity</param>
    void TakeHit(float damage, Collision collider);
}
