using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float timeoutDuration;
    [SerializeField] private float targetChangeDelay;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float maxRandomPathDistance;
    [SerializeField] private float randomPointRadius = 10f;  // Added radius for random points
    private Transform targetLocation;
    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private float currentTimeoutTimer;
    private float currentTargetChangeDelayTimer;

    private void Awake()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void Update()
    {
        if (isPlayerInRange && playerTransform != null)
        {
            if (IsPathValid(playerTransform.position))
            {
                navMeshAgent.destination = playerTransform.position;
            }
            else
            {
                MoveToTargetLocation();
            }
        }
        else
        {
            MoveToTargetLocation();
        }
    }

    private void MoveToTargetLocation()
    {
        if (targetLocation != null)
        {
            if (IsPathValid(targetLocation.position))
            {
                navMeshAgent.destination = targetLocation.position;

                if (Vector3.Distance(transform.position, targetLocation.position) < 1.0f)
                {
                    currentTargetChangeDelayTimer += Time.deltaTime;
                    if (currentTargetChangeDelayTimer >= targetChangeDelay)
                    {
                        targetLocation = GetRandomTargetLocation();
                        currentTargetChangeDelayTimer = 0f;
                    }
                }
            }
            else
            {
                targetLocation = GetRandomTargetLocation();
            }
            currentTimeoutTimer += Time.deltaTime;
            if (currentTimeoutTimer >= timeoutDuration)
            {
                targetLocation = GetRandomTargetLocation();
                currentTimeoutTimer = 0f;
            }
        }
        else
        {
            targetLocation = GetRandomTargetLocation();
        }
    }

    private Transform GetRandomTargetLocation()
    {
        for (int i = 0; i < 10; i++)  // Try up to 10 times to find a valid point
        {
            Vector3 randomDirection = Random.insideUnitSphere * randomPointRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, randomPointRadius, 1))
            {
                if (IsPathValid(hit.position))
                {
                    GameObject tempTarget = new GameObject("TempTarget");
                    tempTarget.transform.position = hit.position;
                    return tempTarget.transform;
                }
            }
        }
        return null;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0f;
        if (path.corners.Length < 2)
            return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }

    private bool IsPathValid(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        bool isValidPath = navMeshAgent.CalculatePath(targetPosition, path);
        return isValidPath && path.status == NavMeshPathStatus.PathComplete;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player!");
            playerTransform = other.transform;
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
        }
    }
}
