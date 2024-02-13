using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain.Layered
{
    /// <summary>
    /// A simple MonoBehaviour which holds the data of an asset created on generation. 
    /// </summary>
    public class TerrainGenerationLayeredAssetDataHolder : MonoBehaviour
    {
        [SerializeField] public TerrainGenerationLayeredAssetData AssetData;
    }
}
