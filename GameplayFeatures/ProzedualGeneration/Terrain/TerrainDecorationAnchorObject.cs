using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Terrain
{
    /// <summary>
    /// Defines an achor object which can serve a specific purpose. (Spawnpoint, Merchant, ...)
    /// </summary>
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
}
