using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPoints : MonoBehaviour
{
    // [SerializeField] public float agentSpeed = 1.0f; // Adjust speed as needed

    private NavMeshAgent agent;
    private int currentTargetIndex;
    private List<TrashBin> targetPoints = new();

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            enabled = false;
        }

        // agent.speed = agentSpeed;
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                if (targetPoints.Count == 0) return;

                var currentBin = targetPoints[currentTargetIndex];
                if (!currentBin.isEmpty)
                {
                    currentBin.EmptyBin();
                    // Move to the next target point
                    NextTarget();
                }
            }
    }

    public void SetNewDestinations(List<TrashBin> newTargetPoints)
    {
        if (newTargetPoints != null && newTargetPoints.Count > 0)
        {
            targetPoints = newTargetPoints;
            currentTargetIndex = 0;
            if (agent == null) return;
            agent.SetDestination(targetPoints[currentTargetIndex].transform.position);
        }
        else
        {
            Debug.LogError("Target points list is empty or null!");
        }
    }

    private void NextTarget()
    {
        if (targetPoints.Count == 0) return;

        currentTargetIndex = (currentTargetIndex + 1) % targetPoints.Count;
        agent.SetDestination(targetPoints[currentTargetIndex].transform.position);
    }
}