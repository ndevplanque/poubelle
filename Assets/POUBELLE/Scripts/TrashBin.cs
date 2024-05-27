using System;
using System.Collections;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public bool isEmpty;
    private Renderer binRenderer;

    private void Start()
    {
        binRenderer = GetComponent<Renderer>();
        UpdateColor();
    }

    public event Action<TrashBin> OnStateChanged;

    public void UpdateColor()
    {
        binRenderer.material.color = isEmpty ? Color.red : Color.green;
    }

    public void EmptyBin()
    {
        isEmpty = true;
        UpdateColor();
        StartCoroutine(ResetBinAfterDelay());
    }

    private IEnumerator ResetBinAfterDelay()
    {
        yield return new WaitForSeconds(15);
        isEmpty = false;
        UpdateColor();
        OnStateChanged?.Invoke(this);
    }
}