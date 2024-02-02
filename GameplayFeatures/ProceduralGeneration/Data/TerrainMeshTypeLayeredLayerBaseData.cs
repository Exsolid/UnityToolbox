using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainMeshTypeLayeredLayerBaseData: ICloneable
    {
        public int CurrentPos;
        public string Name;
        public bool IsBaseLayer;
        public List<TerrainGenerationLayeredAssetBaseData> AssetPlacements;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
