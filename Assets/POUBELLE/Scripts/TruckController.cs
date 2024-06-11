using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 2f; // Vitesse à laquelle le camion tourne
    public float reachThreshold = 0.1f;
    private readonly List<Transform> checkpoints = new();
    private int currentCheckpointIndex;

    private void Start()
    {
        UpdateCheckpoints();
    }

    private void Update()
    {
        if (checkpoints.Count == 0) return;

        var targetCheckpoint = checkpoints[currentCheckpointIndex];

        // Déplacement vers le checkpoint
        var direction = (targetCheckpoint.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotation fluide vers le checkpoint
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Vérifier si le camion est suffisamment proche du checkpoint
        if (Vector3.Distance(transform.position, targetCheckpoint.position) < reachThreshold)
            currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Count;
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

        currentCheckpointIndex = 0;
    }
}