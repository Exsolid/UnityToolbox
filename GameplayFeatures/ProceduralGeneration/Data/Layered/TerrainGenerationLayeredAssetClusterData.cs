using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered
{
    /// <summary>
    /// The data for clustered asset placement.
    /// </summary>
    public class TerrainGenerationLayeredAssetClusterData : TerrainGenerationLayeredAssetBaseData
    {
        public int MinVerticesBetweenPrefabs;
        public float SpawnDecay;
        public List<TerrainGenerationLayeredAssetData> Assets;

        public override object Clone()
        {
            TerrainGenerationLayeredAssetClusterData cloned = (TerrainGenerationLayeredAssetClusterData) base.Clone();

            cloned.Assets = new List<TerrainGenerationLayeredAssetData>();
            foreach (TerrainGenerationLayeredAssetData asset in Assets)
            {
                cloned.Assets.Add((TerrainGenerationLayeredAssetData) asset.Clone());
            }

            return cloned;
        }
    }
}