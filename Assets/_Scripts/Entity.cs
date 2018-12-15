using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : ExtendedMonoBehaviour, IDamageable
{
    public float Health = 10f;
    public float MaxHealth = 10f;
    public bool IsAlive = true;


    /// <summary>
    /// Take hit damage
    /// </summary>
    /// <param name="damage">Damage received</param>
    /// <param name="collider">Colliding entity</param>
    public void TakeHit(float damage, Collision collider)
    {
        Health -= damage;

        if (Health <= 0 && IsAlive)
        {
            Die();
        }
    }

    /// <summary>
    /// Basic entity death
    /// </summary>
    public virtual void Die()
    {
        IsAlive = false;

        // TODO: Should this happen for all game objects?
        Destroy(gameObject);
    }
}
