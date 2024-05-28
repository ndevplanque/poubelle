using System;
using System.Collections;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public bool isEmpty;
    private Renderer binRenderer;
    [SerializeField]
    private Material fullBinMaterial;
    [SerializeField]
    private Material emptyBinMaterial;

    private void Start()
    {
        binRenderer = GetComponent<Renderer>();
        UpdateMaterial();
    }

    public event Action<TrashBin> OnStateChanged;

    public void UpdateMaterial()
    {
        binRenderer.material = isEmpty ? emptyBinMaterial : fullBinMaterial;
        foreach (Transform child in transform)
        {
            var childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.material = binRenderer.material;
            }
        }
    }

    public void EmptyBin()
    {
        isEmpty = true;
        UpdateMaterial();
        StartCoroutine(ResetBinAfterDelay());
    }

    private IEnumerator ResetBinAfterDelay()
    {
        yield return new WaitForSeconds(15);
        isEmpty = false;
        UpdateMaterial();
        OnStateChanged?.Invoke(this);
    }
}