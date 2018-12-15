using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add maximum projectile distance (or damage dropoff rate)

public class Projectile : ExtendedMonoBehaviour
{
    public float MAX_PROJECTILE_DISTANCE = 100f;
    public float MAX_PROJECTILE_LIFETIME = 10f;
    public ProjectileData Data;
    public LayerMask CollisionMask;

    // TODO: Use origin for damage falloff
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

        // Check collisions safely by raycasting (prevents clipping through obstacles).
        //   If a collision will happen within the frame move to within collision distance.
        float frameMoveDistance = Data.Speed * Time.deltaTime;
        float distanceToCollision;
        if (CheckCollisions(frameMoveDistance, out distanceToCollision))
        {
            transform.Translate(Vector3.forward * distanceToCollision);
        }
        else
        {
            transform.Translate(Vector3.forward * frameMoveDistance);
        }

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
        // Only detect collisions on appropriate layers
        if (!UnityExtensions.LayerContains(CollisionMask, collider.gameObject.layer)) return;

        // Apply damage to collider if appropriate
        IDamageable damageableObject = collider.gameObject.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(Data.Damage, collider);
        }

        AudioManager.Instance.PlayEffect(Data.HitSound, transform.position);

        if (Data.HitEffect != null)
        {
            Instantiate(Data.HitEffect, transform.position, Quaternion.identity);
        }

        DestroyProjectile();
    }


    /// <summary>
    /// Check whether a collision will occur in the frame
    /// </summary>
    /// <param name="frameDistance">Distance that will be moved in frame</param>
    public bool CheckCollisions(float frameDistance, out float hitDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, frameDistance, CollisionMask))
        {
            hitDistance = hit.distance;
            return true;
        }

        hitDistance = 0;
        return false;
    }

    /// <summary>
    /// Destroy fired projectile
    /// </summary>
    public void DestroyProjectile()
    {
        // TODO: Play sound effect?

        Destroy(gameObject);
    }
}
