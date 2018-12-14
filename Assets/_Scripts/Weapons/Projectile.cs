using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add maximum projectile distance (or damage dropoff rate)

public class Projectile : ExtendedMonoBehaviour
{
    public float MAX_PROJECTILE_DISTANCE = 100f;
    public float MAX_PROJECTILE_LIFETIME = 10f;
    public ProjectileData Data;

    private Vector3 origin;


    void Start()
    {
        if (Data == null) Data = new ProjectileData();

        origin = transform.position;

        // Projectiles can have limited lifetime (unusual)
        float lifetimeCap = Data.HasLifetime ? Mathf.Min(Data.MaxLifetime, MAX_PROJECTILE_LIFETIME) : MAX_PROJECTILE_LIFETIME;
        Wait(lifetimeCap, () =>
        {
            DestroyProjectile();
        });
    }

    void Update()
    {
        // Projectiles can have limited range (semi-unusual)
        float distanceCap = Data.HasMaxRange ? Mathf.Min(Data.MaxRange, MAX_PROJECTILE_DISTANCE) : MAX_PROJECTILE_DISTANCE;
        float distanceTravelled = Vector3.Distance(origin, transform.position);

        if (distanceTravelled > distanceCap)
        {
            DestroyProjectile();
            return;
        }

        transform.Translate(Vector3.forward * Data.Speed * Time.deltaTime);

        if (GameManager.Instance.DebugMode)
        {
            Debug.DrawLine(origin, transform.position, Color.blue);
            Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
        }
    }

    /// <summary>
    /// Bullet collision with other entities
    /// </summary>
    /// <param name="collider">Colliding entity</param>
    private void OnCollisionEnter(Collision collider)
    {
        // TODO: Trigger damage

        AudioManager.Instance.PlayEffect(Data.HitSound, transform.position);

        if (Data.HitEffect != null)
        {
            Instantiate(Data.HitEffect, transform.position, Quaternion.identity);
        }

        DestroyProjectile();
    }


    /// <summary>
    /// Destroy fired projectile
    /// </summary>
    /// <param name="showHitEffect">Whether hit effect is shown</param>
    public void DestroyProjectile()
    {
        // TODO: Play sound effect?

        Destroy(gameObject);
    }
}
