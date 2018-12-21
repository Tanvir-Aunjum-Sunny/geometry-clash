using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyState
{
    ATTACKING,
    IDLE,
    PURSUING
}


[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Wanderer))]
public class Enemy : ExtendedMonoBehaviour
{
    [Range(0, 1)]
    public float AgentPathRefreshRate = 0.25f;
    [ReadOnly]
    public Damageable Damageable;
    [SerializeField]
    [ReadOnly]
    private EnemyState state;

    // TODO: Move to ScriptableObject
    [Header("Attack")]
    [SerializeField]
    [Range(1, 5)]
    private float attackDamage = 5f;
    [SerializeField]
    [Range(1, 10)]
    private float lungeSpeed = 2f;
    [SerializeField]
    [Range(0.25f, 2)]
    private float attackDistanceThreshold = 0.5f;
    [SerializeField]
    [Range(0.5f, 5)]
    private float timeBetweenAttacks = 1f;
    [SerializeField]
    private Color attackColor = Color.red;

    // Target
    private Transform target;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    // Components
    private new Rigidbody rigidbody;
    private Material skinMaterial;
    private NavMeshAgent pathfinder;
    private Wanderer wanderer;

    private Color originalSkinColor;
    private Coroutine attackCoroutine;
    private Coroutine wanderCoroutine;
    private bool canAttack = true;
    private bool hasAppliedDamageOnAttack = false;
    private float collisionRadius;
    private float targetCollisionRadius;


    void Awake()
    {
        // Components
        Damageable = GetComponent<Damageable>();
        pathfinder = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        collisionRadius = GetComponent<CapsuleCollider>().radius;
        skinMaterial = GetComponent<Renderer>().material;
        wanderer = GetComponent<Wanderer>();
    }

    void Start()
    {
        state = EnemyState.PURSUING;
        originalSkinColor = skinMaterial.color;

        // Enemy (own) death
        Damageable.OnDeath += OnDeath;

        // Target no longer be valid (or exist)
        if (GameManager.Instance.Player != null)
        {
            target = GameManager.Instance.Player.transform;
            hasTarget = true;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            // Target death
            target.GetComponent<Damageable>().OnDeath += OnTargetDeath;

            // Path to target is calculated sporadically for performance
            StartCoroutine(UpdateAgentPath());
        }
        else
        {
            // Enemy should wander if spawning after player death
            wanderCoroutine = StartCoroutine(wanderer.Wander());
        }
    }

    private void Update()
    {
        if (!Damageable.IsAlive) return;

        // Enemies cannot attack consecutively or while idle
        if (hasTarget && canAttack)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            distanceToTarget = distanceToTarget - collisionRadius - targetCollisionRadius;

            // Lunge attack triggered by proxmity to player
            if (distanceToTarget <= attackDistanceThreshold)
            {
                attackCoroutine = StartCoroutine(Attack());

                // Enemies can only attack after previous attack timeout
                canAttack = false;
                hasAppliedDamageOnAttack = false;
                Wait(timeBetweenAttacks, () => {
                    canAttack = true;
                    hasAppliedDamageOnAttack = false;
                });
            }
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        // Enemy can only do damage while attacking
        if (state == EnemyState.ATTACKING)
        {
            // Apply damage to collider if enemy hasn't applied damage yet this attack
            Damageable damageableObject = collider.gameObject.GetComponent<Damageable>();
            if (damageableObject != null && !hasAppliedDamageOnAttack)
            {
                // Prevent further damage from this attack
                hasAppliedDamageOnAttack = true;

                // QUESTION: Does this actually work well?
                // Apply force away from collision
                Vector3 moveDirection = (transform.position - damageableObject.transform.position).normalized;
                rigidbody.AddForce(moveDirection * 50);

                // Attack damage is dealt to both enemy and collision target
                Damageable.TakeDamage(attackDamage, collider.gameObject);
                damageableObject.TakeDamage(attackDamage, gameObject);
            }

            // TODO: Apply force to player
        }
    }


    /// <summary>
    /// Enemy death
    /// </summary>
    /// <param name="killer">Object causing death</param>
    private void OnDeath(GameObject killer)
    {
        // Display death particle effect
        if (Damageable.DeathEffect != null)
        {
            // TODO: Calculate death angle better
            Instantiate(Damageable.DeathEffect, killer.gameObject.transform.position, Quaternion.AngleAxis(90, killer.gameObject.transform.right), TemporaryManager.Instance.TemporaryChildren);
        }
    }

    /// <summary>
    /// Player death stops target tracking
    /// </summary>
    /// <param name="target">Enemy target</param>
    private void OnTargetDeath(GameObject target)
    {
        hasTarget = false;
        state = EnemyState.IDLE;

        if (Damageable.IsAlive)
        {
            // Enemy should wander after target death
            wanderCoroutine = StartCoroutine(wanderer.Wander());
        }
    }

    /// <summary>
    /// Attack the enemies target
    /// </summary>
    /// <returns>Coroutine</returns>
    private IEnumerator Attack()
    {
        if (!hasTarget) yield return null;

        state = EnemyState.ATTACKING;
        pathfinder.enabled = false;
        skinMaterial.color = attackColor;

        // Calculate direction and position of lunge
        Vector3 originalPosition = transform.position;
        Vector3 attackDirection = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - attackDirection * (collisionRadius + targetCollisionRadius - 0.1f);

        float lungePercent = 0;

        // Lunge towards target (lunge halts when collides with player)
        while (lungePercent < 1 && state == EnemyState.ATTACKING)
        {
            lungePercent += Time.deltaTime * lungeSpeed;
            float interpolation = (-Mathf.Pow(lungePercent, 2) + lungePercent) * 4;

            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        state = EnemyState.PURSUING;
        pathfinder.enabled = true;
        skinMaterial.color = originalSkinColor;
    }

    /// <summary>
    /// Update navmesh agent path infrequently (performance)
    /// </summary>
    /// <returns>Coroutine</returns>
    private IEnumerator UpdateAgentPath()
    {
        while (hasTarget)
        {
            if (!Damageable.IsAlive) yield return null;

            // Only update agent path while pursuing
            if (state == EnemyState.PURSUING)
            {
                // Move enemy towards player and within range of attack (but not into player collider)
                Vector3 targetDirection = (target.position - transform.position).normalized;
                float targetOffsetDistance = attackDistanceThreshold / 4;
                targetPosition = target.position - targetDirection * (collisionRadius + targetCollisionRadius + targetOffsetDistance);

                pathfinder.SetDestination(targetPosition);
            }

            yield return new WaitForSeconds(AgentPathRefreshRate);
        }
    }


    private void OnDrawGizmos()
    {
        if (GameManager.Instance == null || !GameManager.Instance.DebugMode) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.25f);
    }
}
