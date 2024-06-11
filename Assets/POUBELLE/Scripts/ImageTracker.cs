using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    public GameObject[] ArPrefabs;
    public GameObject Truck; // Reference to the truck prefab

    private readonly List<GameObject> ARObjects = new();
    private ARTrackedImageManager trackedImages;
    private TruckController truckController; // Reference to the truck's controller script
    private bool truckInitialized;

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
                Debug.Log($"Trackable image found: {trackedImage.referenceImage.name}");
                var newPrefab = Instantiate(arPrefab, trackedImage.transform);

                ARObjects.Add(newPrefab);

                // Initialize the truck only when the first tracked prefab is added  
                if (!truckInitialized)
                {
                    InitializeTruck(newPrefab);
                    truckInitialized = true;
                }
                else
                {
                    // Update checkpoints in the truck controller if truck already initialized  
                    truckController.UpdateCheckpoints();
                }
            }

        foreach (var trackedImage in eventArgs.updated)
        foreach (var arObject in ARObjects)
            if (arObject.name == trackedImage.referenceImage.name)
                arObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }

    private void InitializeTruck(GameObject parentObject)
    {
        // Instantiate the truck prefab at the same position as the first tracked AR prefab  
        var truckInstance = Instantiate(Truck, parentObject.transform.position, parentObject.transform.rotation);

        // Get the truck controller component and update its checkpoints list
        truckController = truckInstance.GetComponent<TruckController>();
        truckController.UpdateCheckpoints();
    }
}