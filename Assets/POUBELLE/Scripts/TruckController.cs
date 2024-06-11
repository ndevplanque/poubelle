using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 2f; // Vitesse à laquelle le camion tourne
    public float reachThreshold = 0.1f;
    private readonly List<Transform> checkpoints = new();
    private Transform currentCheckpoint;
    private List<Transform> remainingCheckpoints = new();

    private void Start()
    {
        UpdateCheckpoints();
        SetClosestCheckpoint();
    }

    private void Update()
    {
        if (currentCheckpoint == null || checkpoints.Count == 0) return;

        // Déplacement vers le checkpoint
        var direction = (currentCheckpoint.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotation fluide vers le checkpoint
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Vérifier si le camion est suffisamment proche du checkpoint
        if (Vector3.Distance(transform.position, currentCheckpoint.position) < reachThreshold)
        {
            HandleCheckpointReached();
            SetClosestCheckpoint();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var checkpoint in checkpoints) Gizmos.DrawSphere(checkpoint.position, 0.01f);
    }

    public void UpdateCheckpoints()
    {
        checkpoints.Clear();
        var checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");

        foreach (var checkpointObject in checkpointObjects) checkpoints.Add(checkpointObject.transform);

        ResetRemainingCheckpoints();
    }

    private void SetClosestCheckpoint()
    {
        if (remainingCheckpoints.Count == 0) ResetRemainingCheckpoints();

        var closestDistance = Mathf.Infinity;
        Transform closestCheckpoint = null;

        foreach (var checkpoint in remainingCheckpoints)
        {
            var distance = Vector3.Distance(transform.position, checkpoint.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCheckpoint = checkpoint;
            }
        }

        currentCheckpoint = closestCheckpoint;

        if (currentCheckpoint != null) remainingCheckpoints.Remove(currentCheckpoint);
    }

    private void HandleCheckpointReached()
    {
        if (currentCheckpoint != null && remainingCheckpoints.Contains(currentCheckpoint))
            remainingCheckpoints.Remove(currentCheckpoint);
    }

    private void ResetRemainingCheckpoints()
    {
        remainingCheckpoints = new List<Transform>(checkpoints);
    }
}