using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    public GameObject[] ArPrefabs;
    public GameObject Truck;

    private readonly List<GameObject> ARObjects = new();
    private bool floorDetected;
    private float floorYPosition;
    private ARPlaneManager planeManager;
    private ARTrackedImageManager trackedImages;
    private TruckController truckController;
    private bool truckInitialized;

    private void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
        planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        foreach (var arPrefab in ArPrefabs)
            if (trackedImage.referenceImage.name == arPrefab.name)
            {
                Debug.Log($"Trackable image found: {trackedImage.referenceImage.name}");

                // Use the detected floor height if available
                if (floorDetected)
                {
                    var newPosition = new Vector3(trackedImage.transform.position.x, floorYPosition,
                        trackedImage.transform.position.z);
                    var newPrefab = Instantiate(arPrefab, newPosition, trackedImage.transform.rotation);
                    ARObjects.Add(newPrefab);

                    // Initialize the truck only when the first tracked prefab is added
                    if (!truckInitialized)
                    {
                        InitializeTruck(newPosition, trackedImage.transform.rotation);
                        truckInitialized = true;
                    }
                    else
                    {
                        // Update checkpoints in the truck controller if truck already initialized
                        truckController.UpdateCheckpoints();
                    }
                }

                break;
            }

        foreach (var trackedImage in eventArgs.updated)
        foreach (var arObject in ARObjects)
            if (arObject.name == trackedImage.referenceImage.name)
                arObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        // Handle the addition of new planes
        foreach (var plane in eventArgs.added)
            if (!floorDetected)
            {
                // Detect the floor by assuming the first detected plane is the floor.
                // You might want to refine this logic to ensure you are detecting the correct floor plane.
                floorYPosition = plane.transform.position.y;
                floorDetected = true;
                Debug.Log("Floor detected at height: " + floorYPosition);
            }
    }

    private void InitializeTruck(Vector3 position, Quaternion rotation)
    {
        // Instantiate the truck prefab at the detected floor height
        var truckInstance = Instantiate(Truck, position, rotation);

        // Get the truck controller component and update its checkpoints list
        truckController = truckInstance.GetComponent<TruckController>();
        truckController.UpdateCheckpoints();
    }
}