using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : ExtendedMonoBehaviour
{
    public bool IsAlive { get; private set; } = true;
    public float Health = 10f;
    public float MaxHealth = 10f;
    public GameObject HitEffect;
    public AudioClip HitSound;

    #region Events
    public Action<float> OnDamage;
    public Action OnDeath;
    #endregion


    /// <summary>
    /// Take damage from a source
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    /// <param name="damager">Object inflicting damage</param>
    public void TakeDamage(float damage, GameObject damager)
    {
        if (!IsAlive) return;

        // Raise OnDamage event
        OnDamage?.Invoke(damage);

        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Kill a damageable object
    /// </summary>
    public void Die()
    {
        if (!IsAlive) return;

        IsAlive = false;
        Health = 0f;

        // Raise OnDeath event
        OnDeath?.Invoke();

        Destroy(gameObject);
    }
}
