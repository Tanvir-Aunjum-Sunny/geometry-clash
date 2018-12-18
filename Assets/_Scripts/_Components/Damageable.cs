using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : ExtendedMonoBehaviour
{
    public bool IsAlive { get; private set; } = true;
    public float Health = 10f;
    public float MaxHealth = 10f;

    #region Events
    public Action<float> OnDamage;
    public Action OnDeath;

    // [Serializable]
    // public class DamageEvent : UnityEvent<float> { }
    // public DamageEvent OnDamageUnity;
    // public UnityEvent OnDeathUnity;
    #endregion


    public void TakeDamage(float damage)
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
