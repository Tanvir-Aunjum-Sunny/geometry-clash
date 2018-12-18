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
    }

    void Start()
    {
        StartCoroutine(UpdateAgentPath());
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
