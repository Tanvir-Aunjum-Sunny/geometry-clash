using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Wanderer : ExtendedMonoBehaviour
{
    public Vector2 WanderPointRefreshRate;
    [Range(1, 20)]
    public float WanderRadius = 5;

    private NavMeshAgent pathfinder;


    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
    }


    /// <summary>
    /// Randomly wander as long as coroutine runs
    /// </summary>
    /// <returns>Coroutine</returns>
    public IEnumerator Wander()
    {
        while (true)
        {
            if (!pathfinder.enabled) yield return null;

            Vector3 newDestination = RandomPointInNavSphere(transform.position, WanderRadius);
            pathfinder.SetDestination(newDestination);

            float randomWaitTime = Random.Range(WanderPointRefreshRate.x, WanderPointRefreshRate.y);
            yield return new WaitForSeconds(randomWaitTime);
        }
    }

    /// <summary>
    /// Find a random accessible navigation point
    /// </summary>
    /// <param name="origin">Wander agent origin</param>
    /// <param name="radius">Wander radius</param>
    /// <param name="layerMask">Wander navigable layer mask</param>
    /// <returns>Randomized navigation point</returns>
    public static Vector3 RandomPointInNavSphere(Vector3 origin, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += origin;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 1);

        return hit.position;
    }
}
