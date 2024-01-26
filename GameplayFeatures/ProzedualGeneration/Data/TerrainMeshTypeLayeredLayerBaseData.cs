using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Data
{
    public class TerrainMeshTypeLayeredLayerBaseData
    {
        public int CurrentPos;
        public string Name;
        public bool IsBaseLayer;
        public List<TerrainGenerationLayeredAssetBaseData> AssetPlacements;
    }
}
