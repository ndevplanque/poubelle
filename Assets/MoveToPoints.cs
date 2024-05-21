using UnityEngine;
using UnityEngine.AI;

public class MoveToPoints : MonoBehaviour
{
    public Transform targetPoint; // The point where the object needs to go
    public Color closeColor = Color.yellow; // Color to change to when the agent is close to the point
    public float closeDistance = 1.0f; // Distance threshold for when the agent is considered close to the point
    private NavMeshAgent agent;
    private Renderer targetRenderer;
    private Color originalColor;

    void Start()
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

        // Get reference to the renderer component of the target point
        targetRenderer = targetPoint.GetComponent<Renderer>();

        // Check if the targetRenderer component exists
        if (targetRenderer == null)
        {
            Debug.LogError("Renderer component not found on target point!");
            enabled = false; // Disable this script
            return;
        }

        // Store the original color of the target point
        originalColor = targetRenderer.material.color;

        // Set the destination of the NavMeshAgent to the target point
        if (targetPoint != null)
        {
            agent.SetDestination(targetPoint.position);
        }
        else
        {
            Debug.LogError("Target point not assigned!");
            enabled = false; // Disable this script
        }
    }

    void Update()
    {
        // Check if the agent is close to the target point
        if (Vector3.Distance(transform.position, targetPoint.position) < closeDistance)
        {
            // Change the color of the target point to the closeColor
            targetRenderer.material.color = closeColor;
        }
        else
        {
            // Restore the original color of the target point
            targetRenderer.material.color = originalColor;
        }
    }
    
    public void SetNewDestination(Transform newTargetPoint)
    {
        if (newTargetPoint != null)
        {
            targetPoint = newTargetPoint;
            agent.SetDestination(targetPoint.position);
        }
        else
        {
            Debug.LogError("Target point not assigned!");
        }
    }

}
