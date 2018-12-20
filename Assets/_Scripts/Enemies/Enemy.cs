using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : ExtendedMonoBehaviour
{
    [Range(0, 1)]
    public float AgentPathRefreshRate = 0.25f;

    [ReadOnly] public Damageable Damageable;

    private Transform target;
    private NavMeshAgent pathfinder;


    void Awake()
    {
        Damageable = GetComponent<Damageable>();
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameManager.Instance.Player.transform;

        Damageable.OnDeath += OnDeath;
    }

    void Start()
    {
        StartCoroutine(UpdateAgentPath());
    }


    /// <summary>
    /// Enemy death
    /// </summary>
    /// <param name="killer">Object causing death</param>
    private void OnDeath(GameObject killer)
    {
        if (Damageable.DeathEffect != null)
        {
            Instantiate(Damageable.DeathEffect, killer.gameObject.transform.position, Quaternion.AngleAxis(90, killer.gameObject.transform.right), TemporaryManager.Instance.TemporaryChildren);
        }
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

            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            pathfinder.SetDestination(targetPosition);

            yield return new WaitForSeconds(AgentPathRefreshRate);
        }
    }
}
