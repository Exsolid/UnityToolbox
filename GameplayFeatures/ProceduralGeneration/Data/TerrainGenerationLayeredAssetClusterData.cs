using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainGenerationLayeredAssetClusterData : TerrainGenerationLayeredAssetBaseData
    {
        public int MinVerticesBetweenPrefabs;
        public float SpawnDecay;
        public List<TerrainGenerationLayeredAssetData> Assets;
    }
}