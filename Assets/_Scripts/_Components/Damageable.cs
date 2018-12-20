using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : ExtendedMonoBehaviour
{
    public bool IsAlive { get; private set; } = true;
    public float Health = 10f;
    public float MaxHealth = 10f;

    [Header("Effects")]
    public GameObject DamageEffect;
    public AudioClip DamageSound;
    public GameObject DeathEffect;

    #region Events
    public Action<float, GameObject> OnDamage;
    public Action<GameObject> OnDeath;
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
        OnDamage?.Invoke(damage, damager);

        Health -= damage;
        if (Health <= 0)
        {
            Die(damager);
        }
    }

    /// <summary>
    /// Kill a damageable object
    /// <param name="killer">Object causing death</param>
    /// </summary>
    public void Die(GameObject killer)
    {
        if (!IsAlive) return;

        IsAlive = false;
        Health = 0f;

        // Raise OnDeath event
        OnDeath?.Invoke(killer);

        Destroy(gameObject);
    }
}
