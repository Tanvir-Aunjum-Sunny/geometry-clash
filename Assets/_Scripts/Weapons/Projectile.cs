using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Projectiles deal damage from weapons
/// </summary>
public class Projectile : ExtendedMonoBehaviour
{
    public float MAX_PROJECTILE_DISTANCE = 100f;
    public float MAX_PROJECTILE_LIFETIME = 10f;
    public ProjectileData Data;
    public LayerMask CollisionMask;

    // TODO: Use origin for damage falloff (distance-based)
    private Vector3 origin;


    void Start()
    {
        if (Data == null) Data = new ProjectileData();

        origin = transform.position;

        // Find all colliders the projectile intersects with initiaally
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, CollisionMask);
        if (initialCollisions.Length > 0)
        {
            OnCollision(initialCollisions[0].gameObject);
        }

        // Projectiles can have limited lifetime (unusual)
        float lifetimeCap = Data.HasLifetime ? Mathf.Min(Data.MaxLifetime, MAX_PROJECTILE_LIFETIME) : MAX_PROJECTILE_LIFETIME;
        Wait(lifetimeCap, () =>
        {
            Destroy(gameObject);
        });
    }

    void Update()
    {
        // Projectiles can have limited range (semi-unusual)
        float distanceCap = Data.HasMaxRange ? Mathf.Min(Data.MaxRange, MAX_PROJECTILE_DISTANCE) : MAX_PROJECTILE_DISTANCE;
        float distanceTravelled = Vector3.Distance(origin, transform.position);

        if (distanceTravelled > distanceCap)
        {
            Destroy(gameObject);
            return;
        }

        // Check collisions safely by raycasting (prevents clipping through obstacles).
        //   If a collision will happen within the frame move to within collision distance.
        float frameMoveDistance = Data.Speed * Time.deltaTime;
        if (CheckCollisions(frameMoveDistance, out float distanceToCollision))
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

    private void OnCollisionEnter(Collision collider)
    {
        OnCollision(collider.gameObject);
    }


    /// <summary>
    /// Handle bullet collision with other entities
    /// </summary>
    /// <param name="collider">Colliding game object</param>
    private void OnCollision(GameObject collider)
    {
        // Only detect collisions on appropriate layers
        if (!UnityExtensions.LayerContains(CollisionMask, collider.layer)) return;

        // Apply damage to collider if appropriate
        Damageable damageableObject = collider.GetComponent<Damageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(Data.Damage, gameObject);
        }

        // TODO: Does this logic belong in Damageable component?
        // Enemies react differently to being hit
        Enemy enemy = collider.GetComponent<Enemy>();
        if (enemy == null)
        {
            if (Data.HitEffect != null)
            {
                // Use collider material for hit effect
                Material collidingMaterial = collider.GetComponent<Renderer>().material;
                Data.HitEffect.GetComponent<Renderer>().material = collidingMaterial;

                Instantiate(Data.HitEffect, transform.position, Quaternion.identity, TemporaryManager.Instance.TemporaryChildren);
            }

            // QUESTION: Should audio be moved to hit object (in "Hit-able" component)?
            AudioManager.Instance.PlayEffect(Data.HitSound, transform.position);
        }
        else
        {
            if (enemy.Damageable.DamageEffect != null)
            {
                // Use collider material for hit effect
                Material collidingMaterial = collider.GetComponent<Renderer>().material;
                enemy.Damageable.DamageEffect.GetComponent<Renderer>().material = collidingMaterial;

                Instantiate(enemy.Damageable.DamageEffect, transform.position, Quaternion.identity, TemporaryManager.Instance.TemporaryChildren);
            }

            AudioManager.Instance.PlayEffect(enemy.Damageable.DamageSound, transform.position);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Check whether a collision will occur within the frame (prevents collision clipping)
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
}
