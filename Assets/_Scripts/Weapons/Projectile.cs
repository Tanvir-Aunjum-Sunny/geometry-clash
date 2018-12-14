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
            DestroyProjectile(false);
        });
    }

    void Update()
    {
        // Projectiles can have limited range (semi-unusual)
        float distanceCap = Data.HasMaxRange ? Mathf.Min(Data.MaxRange, MAX_PROJECTILE_DISTANCE) : MAX_PROJECTILE_DISTANCE;
        float distanceTravelled = Vector3.Distance(origin, transform.position);

        if (distanceTravelled > distanceCap)
        {
            DestroyProjectile(false);
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
        AudioManager.Instance.PlayEffect(Data.HitSound, transform.position);

        // TODO: Trigger damage

        Destroy(gameObject);
    }


    /// <summary>
    /// Destroy fired projectile
    /// </summary>
    /// <param name="showEffect">Whether death effect is shown</param>
    public void DestroyProjectile(bool showEffect = false)
    {
        // Display effect when projectile is destroyed
        if (showEffect && Data.DestroyEffect != null)
        {
            Instantiate(Data.DestroyEffect, transform.position, Quaternion.identity);
        }

        // TODO: Play sound effect?

        Destroy(gameObject);
    }
}
