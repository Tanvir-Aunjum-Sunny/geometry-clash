using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity
{
    [Range(0, 1)]
    public float AgentPathRefreshRate = 0.25f;

    private Transform target;
    private NavMeshAgent pathfinder;


    protected void Start()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameManager.Instance.Player.transform;

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
            if (!IsAlive) yield return null;

            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            pathfinder.SetDestination(targetPosition);

            yield return new WaitForSeconds(AgentPathRefreshRate);
        }
    }
}
