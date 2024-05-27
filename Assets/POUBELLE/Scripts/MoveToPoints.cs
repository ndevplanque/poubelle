using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.AI;

public class MoveToPoints : MonoBehaviour  
{  
    private NavMeshAgent agent;  
    private List<Transform> targetPoints = new List<Transform>();
    private int currentTargetIndex = 0;

    private void Start()  
    {  
        // Get reference to the NavMeshAgent component attached to this GameObject  
        agent = GetComponent<NavMeshAgent>();

        // Check if the NavMeshAgent component exists  
        if (agent == null)  
        {  
            Debug.LogError("NavMeshAgent component not found!");  
            enabled = false; // Disable this script  
            return;  
        }
    }

    private void Update()
    {
        // Check if the agent has reached its current destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                // Move to the next target point
                NextTarget();
            }
        }
    }

    public void SetNewDestinations(List<Transform> newTargetPoints)  
    {  
        if (newTargetPoints != null && newTargetPoints.Count > 0)  
        {  
            targetPoints = newTargetPoints;  
            currentTargetIndex = 0;
            agent.SetDestination(targetPoints[currentTargetIndex].position);  
        }  
        else  
        {  
            Debug.LogError("Target points list is empty or null!");  
        }  
    }

    private void NextTarget()
    {
        // Cycle to the next target point
        currentTargetIndex = (currentTargetIndex + 1) % targetPoints.Count;
        agent.SetDestination(targetPoints[currentTargetIndex].position);
    }
}