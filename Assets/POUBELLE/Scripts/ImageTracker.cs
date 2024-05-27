using System.Collections;  
using System.Collections.Generic;  
using Unity.AI.Navigation;  
using UnityEngine;

using UnityEngine.XR.ARFoundation;  
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour  
{  
    private ARTrackedImageManager trackedImages;  
    private MoveToPoints truckMovement;  
    public GameObject[] ArPrefabs;

    List<GameObject> ARObjects = new List<GameObject>();

    void Start()  
    {  
        // Assurez-vous que l'agent est attaché à votre camion.  
        truckMovement = GameObject.Find("Truck").GetComponent<MoveToPoints>();  
    }

    
    void Awake()  
    {  
        trackedImages = GetComponent<ARTrackedImageManager>();  
    }

    void OnEnable()  
    {  
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;  
    }

    void OnDisable()  
    {  
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;  
    }


    // Event Handler  
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)  
    {  
        //Create object based on image tracked  
        foreach (var trackedImage in eventArgs.added)  
        {  
            foreach (var arPrefab in ArPrefabs)  
            {  
                if(trackedImage.referenceImage.name == arPrefab.name)  
                {  
                    var newPrefab = Instantiate(arPrefab, trackedImage.transform);  
                    
                    // Build the NavMesh  
                    NavMeshSurface surface = newPrefab.GetComponent<NavMeshSurface>();  
                    if (surface != null)  
                    {  
                        surface.BuildNavMesh();  
                    }

                    // Collecte toutes les poubelles dans une liste
                    var trashObjects = FindAllTrashObjects(newPrefab);
                    if (trashObjects.Count > 0)
                    {
                        // Définir la destination du camion sur chaque poubelle
                        truckMovement.SetNewDestinations(trashObjects);
                    }
                    
                    ARObjects.Add(newPrefab);  
                }  
            }  
        }  
        
        //Update tracking position  
        foreach (var trackedImage in eventArgs.updated)  
        {  
            foreach (var gameObject in ARObjects)  
            {  
                if(gameObject.name == trackedImage.referenceImage.name)  
                {  
                    gameObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);  
                }  
            }  
        }  
    }

    // Fonction pour rechercher tous les objets avec le tag "Trash"
    private List<Transform> FindAllTrashObjects(GameObject parentObject)
    {
        List<Transform> trashObjects = new List<Transform>();
        foreach (Transform child in parentObject.transform)
        {
            if (child.CompareTag("Trash"))
            {
                trashObjects.Add(child);
            }
            else
            {
                // Recherche récursive dans tous les descendants
                trashObjects.AddRange(FindAllTrashObjects(child.gameObject));
            }
        }
        return trashObjects;
    }
}
