using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TrashBin : MonoBehaviour
{
    public bool isEmpty;

    [SerializeField] private int id;

    [SerializeField] private Material fullBinMaterial;

    [SerializeField] private Material emptyBinMaterial;

    private readonly string apiURL = "https://gomiapi-47.localcan.dev/trash";

    private Renderer binRenderer;

    private void Start()
    {
        binRenderer = GetComponent<Renderer>();
        UpdateMaterial();

        // Start the coroutine to check bin state every second  
        StartCoroutine(CheckBinStatePeriodically());
    }

    private IEnumerator CheckBinStatePeriodically()
    {
        while (true)
        {
            yield return CheckBinState();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator CheckBinState()
    {
        using (var request = UnityWebRequest.Get(apiURL + "/" + id))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}, URL: {apiURL}/{id}");
            }
            else
            {
                // Parse the JSON response using Unity's JsonUtility  
                var jsonResponse = request.downloadHandler.text;
                var binState = JsonUtility.FromJson<BinStateResponse>(jsonResponse);

                changeTrashState(!binState.full);
            }
        }
    }

    public event Action<TrashBin> OnStateChanged;

    public void UpdateMaterial()
    {
        binRenderer.material = isEmpty ? emptyBinMaterial : fullBinMaterial;
        foreach (Transform child in transform)
        {
            var childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null) childRenderer.material = binRenderer.material;
        }
    }

    private void changeTrashState(bool empty)
    {
        isEmpty = empty;
        UpdateMaterial();

        OnStateChanged?.Invoke(this); // Ensure state change events are invoked every time the state changes
    }

    public void EmptyBin()
    {
        StartCoroutine(SendApiRequest($"{apiURL}/{id}/empty", "POST"));
    }

    public void FillBin()
    {
        StartCoroutine(SendApiRequest($"{apiURL}/{id}/full", "POST"));
    }

    private IEnumerator SendApiRequest(string url, string method)
    {
        var request = new UnityWebRequest(url, method);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Request failed: {request.error}, URL: {url}, Method: {method}");
        }
        else
        {
            // Debug.Log($"Request completed successfully: {request.downloadHandler.text}");
            // Update bin state manually after API call  
            yield return CheckBinState();
        }
    }

    [Serializable]
    public class BinStateResponse
    {
        public bool full;
        public bool open;
    }
}