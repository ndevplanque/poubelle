using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    public GameObject[] ArPrefabs;
    public GameObject Truck; // Reference to the truck prefab

    private readonly List<GameObject> ARObjects = new();
    private ARTrackedImageManager trackedImages;
    private bool truckInitialized; // To track if the truck is already initialized
    private MoveToPoints truckMovement;

    private void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
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

                ARObjects.Add(newPrefab);

                // Initialize the truck only when the first tracked prefab is added
                if (!truckInitialized)
                {
                    InitializeTruck(newPrefab);
                    truckInitialized = true;
                }

                // if (trashBins.Count > 0) truckMovement.SetNewDestinations(trashBins.FindAll(bin => !bin.isEmpty));
            }

        foreach (var trackedImage in eventArgs.updated)
        foreach (var gameObject in ARObjects)
            if (gameObject.name == trackedImage.referenceImage.name)
                gameObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }

    private void InitializeTruck(GameObject parentObject)
    {
        // Instantiate the truck prefab at the same position as the first tracked AR prefab
        var truckInstance = Instantiate(Truck, parentObject.transform.position, parentObject.transform.rotation);
        truckMovement = truckInstance.GetComponent<MoveToPoints>();

        // Set initial destinations for the truck
        var bins = FindAllTrashObjects(parentObject);
        truckMovement.SetNewDestinations(bins.FindAll(bin => !bin.isEmpty));
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