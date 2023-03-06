using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDecorationAnchorObject : MonoBehaviour
{
    private void OnValidate()
    {
        foreach(Transform child in transform)
        {
            if(child.GetComponent<TerrainDecorationAnchorObject>() == null)
            {
                child.gameObject.AddComponent<TerrainDecorationAnchorObject>();
                AddRecursive(child);
            }
        }
    }

    private void AddRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<TerrainDecorationAnchorObject>() == null)
            {
                child.gameObject.AddComponent<TerrainDecorationAnchorObject>();
                AddRecursive(child);
            }
        }
    }
}