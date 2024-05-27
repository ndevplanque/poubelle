using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    public GameObject[] ArPrefabs;

    private readonly List<GameObject> ARObjects = new();
    private ARTrackedImageManager trackedImages;
    private MoveToPoints truckMovement;


    private void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
    }

    private void Start()
    {
        // Assurez-vous que l'agent est attaché à votre camion.  
        truckMovement = GameObject.Find("Truck").GetComponent<MoveToPoints>();
    }

    private void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        foreach (var arPrefab in ArPrefabs)
            if (trackedImage.referenceImage.name == arPrefab.name)
            {
                var newPrefab = Instantiate(arPrefab, trackedImage.transform);

                // Build the NavMesh
                var surface = newPrefab.GetComponent<NavMeshSurface>();
                if (surface != null)
                    surface.BuildNavMesh();

                var trashBins = FindAllTrashObjects(newPrefab);
                foreach (var bin in trashBins) bin.OnStateChanged += HandleBinStateChanged;

                if (trashBins.Count > 0) truckMovement.SetNewDestinations(trashBins.FindAll(bin => !bin.isEmpty));

                ARObjects.Add(newPrefab);
            }

        foreach (var trackedImage in eventArgs.updated)
        foreach (var gameObject in ARObjects)
            if (gameObject.name == trackedImage.referenceImage.name)
                gameObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }

    // Function to search for all TrashBin objects
    private List<TrashBin> FindAllTrashObjects(GameObject parentObject)
    {
        var trashBins = new List<TrashBin>();
        foreach (Transform child in parentObject.transform)
        {
            var trashBin = child.GetComponent<TrashBin>();
            if (trashBin != null)
                trashBins.Add(trashBin);
            else
                trashBins.AddRange(FindAllTrashObjects(child.gameObject));
        }

        return trashBins;
    }

    // Event handler for bin state changes
    private void HandleBinStateChanged(TrashBin bin)
    {
        var bins = FindAllTrashObjects(gameObject);
        truckMovement.SetNewDestinations(bins.FindAll(b => !b.isEmpty));
    }
}