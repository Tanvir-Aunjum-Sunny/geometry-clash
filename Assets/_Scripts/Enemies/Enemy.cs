using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyState
{
    ATTACKING,
    DEAD,
    IDLE,
    PURSUING
}


[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : ExtendedMonoBehaviour
{
    [Range(0, 1)]
    public float AgentPathRefreshRate = 0.25f;
    [ReadOnly]
    public Damageable Damageable;
    [SerializeField]
    [ReadOnly]
    private EnemyState state;

    [Header("Attack")]
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

    private Transform target;
    private Material skinMaterial;
    private Color originalSkinColor;
    private NavMeshAgent pathfinder;
    private bool canAttack = true;
    private float collisionRadius;
    private float targetCollisionRadius;


    void Awake()
    {
        Damageable = GetComponent<Damageable>();
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameManager.Instance.Player.transform;
        collisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        skinMaterial = GetComponent<Renderer>().material;
        originalSkinColor = skinMaterial.color;
    }

    void Start()
    {
        state = EnemyState.PURSUING;

        Damageable.OnDeath += OnDeath;

        StartCoroutine(UpdateAgentPath());
    }

    private void Update()
    {
        if (!Damageable.IsAlive) return;

        if (canAttack)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            distanceToTarget = distanceToTarget - collisionRadius - targetCollisionRadius;

            // Proxmity to player triggers a lunge
            if (distanceToTarget <= attackDistanceThreshold)
            {
                StartCoroutine(Attack());

                // Enemies can only attack after previous attack timeout
                canAttack = false;
                Wait(timeBetweenAttacks, () => {
                    canAttack = true;
                });
            }
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
            Instantiate(Damageable.DeathEffect, killer.gameObject.transform.position, Quaternion.AngleAxis(90, killer.gameObject.transform.right), TemporaryManager.Instance.TemporaryChildren);
        }
    }

    /// <summary>
    /// Attack the enemies target
    /// </summary>
    /// <returns>Coroutine</returns>
    private IEnumerator Attack()
    {
        state = EnemyState.ATTACKING;
        pathfinder.enabled = false;
        skinMaterial.color = attackColor;

        // Lunge towards target (with only fractional overlap to trigger damage)
        Vector3 originalPosition = transform.position;
        Vector3 attackDirection = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - attackDirection * (collisionRadius + targetCollisionRadius - 0.1f);

        float lungePercent = 0;

        while (lungePercent < 1)
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
        while (target != null)
        {
            if (!Damageable.IsAlive) yield return null;

            if (state != EnemyState.ATTACKING)
            {
                // Move enemy towards player and within range of attack (but not into player collider)
                Vector3 targetDirection = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - targetDirection * (collisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);

                pathfinder.SetDestination(targetPosition);
            }

            yield return new WaitForSeconds(AgentPathRefreshRate);
        }
    }
}
