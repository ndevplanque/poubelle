using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 2f; // Vitesse à laquelle le camion tourne  
    public float reachThreshold = 0.1f;
    public float disableDuration = 3; // Durée pendant laquelle le checkpoint est désactivé
    public float stopDuration = 3f; // Durée d'arrêt pour vider la poubelle

    [SerializeField] private float trashDetectionRadius = 2f; // Rayon de détection des poubelles

    private readonly List<Transform> checkpoints = new();
    private Transform currentCheckpoint;
    private readonly float effectiveDetectionDistance = 0.1f; // Distance effective pour détecter une poubelle voisine
    private bool isStopped;
    private List<Transform> remainingCheckpoints = new();

    private void Start()
    {
        UpdateCheckpoints();
        SetClosestCheckpoint();
    }

    private void Update()
    {
        if (currentCheckpoint == null || checkpoints.Count == 0 || isStopped) return;

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
        if (checkpoints == null) return;

        foreach (var checkpoint in checkpoints)
        {
            var checkpointState = checkpoint.GetComponent<CheckpointState>();
            Gizmos.color = checkpointState != null && !checkpointState.IsActive ? Color.red : Color.blue;
            Gizmos.DrawSphere(checkpoint.position, 0.01f); // Laisser à 0.01f pour éviter de cacher le checkpoint  
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            var trashBin = other.GetComponent<TrashBin>();
            if (trashBin != null && !trashBin.isEmpty) StartCoroutine(StopAndEmptyBin(trashBin));
        }
    }

    public void UpdateCheckpoints()
    {
        checkpoints.Clear();
        var checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");

        foreach (var checkpointObject in checkpointObjects)
        {
            checkpoints.Add(checkpointObject.transform);
            if (checkpointObject.GetComponent<CheckpointState>() == null)
                checkpointObject.gameObject.AddComponent<CheckpointState>(); // Ajout du composant CheckpointState  
        }

        ResetRemainingCheckpoints();
    }

    private void SetClosestCheckpoint()
    {
        if (remainingCheckpoints.Count == 0)
            ResetRemainingCheckpoints();

        var closestDistance = Mathf.Infinity;
        Transform closestCheckpoint = null;

        foreach (var checkpoint in remainingCheckpoints)
        {
            var checkpointState = checkpoint.GetComponent<CheckpointState>();
            if (checkpointState == null || !checkpointState.IsActive) continue;

            var distance = Vector3.Distance(transform.position, checkpoint.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCheckpoint = checkpoint;
            }
        }

        currentCheckpoint = closestCheckpoint;

        // Si on a trouvé un checkpoint, on l'enlève des checkpoints restants  
        if (currentCheckpoint != null)
            remainingCheckpoints.Remove(currentCheckpoint);
        else
            // Si aucun checkpoint actif n'a été trouvé, réessayer un peu plus tard  
            Invoke(nameof(SetClosestCheckpoint), 1f); // Réessayer après 1 seconde  
    }

    private void HandleCheckpointReached()
    {
        if (currentCheckpoint != null)
        {
            var checkpointState = currentCheckpoint.GetComponent<CheckpointState>();

            // Vérifiez s'il y a des poubelles à vider dans le rayon de détection
            var colliders = Physics.OverlapSphere(currentCheckpoint.position, trashDetectionRadius);

            foreach (var col in colliders)
                if (col.CompareTag("Trash"))
                {
                    var trashBin = col.GetComponent<TrashBin>();
                    if (trashBin != null && !trashBin.isEmpty)
                    {
                        // Vérifier la distance réelle entre le camion et la poubelle
                        var actualDistance = Vector3.Distance(transform.position, col.transform.position);
                        if (actualDistance < effectiveDetectionDistance)
                        {
                            StartCoroutine(StopAndEmptyBin(trashBin));
                            break; // Ne vider qu'une seule poubelle à la fois
                        }
                    }
                }

            checkpointState.DeactivateForDuration(disableDuration);
        }
    }

    private void ResetRemainingCheckpoints()
    {
        remainingCheckpoints = new List<Transform>(checkpoints);
    }

    private IEnumerator StopAndEmptyBin(TrashBin trashBin)
    {
        isStopped = true;
        
        // Attendre un certain temps pendant que le camion est arrêté
        yield return new WaitForSeconds(stopDuration);

        trashBin.EmptyBin(); // Vider la poubelle
        yield return new WaitUntil(() => trashBin.isEmpty);

        isStopped = false; // Reprendre le mouvement
    }
}

public class CheckpointState : MonoBehaviour
{
    public bool IsActive { get; private set; } = true;

    public void DeactivateForDuration(float duration)
    {
        if (IsActive) StartCoroutine(DeactivateCoroutine(duration));
    }

    private IEnumerator DeactivateCoroutine(float duration)
    {
        IsActive = false;
        yield return new WaitForSeconds(duration);
        IsActive = true;
    }
}