using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered
{
    /// <summary>
    /// The data for a layer of the layered mesh generation type.
    /// </summary>
    public class TerrainMeshTypeLayeredLayerBaseData: ICloneable
    {
        public int CurrentPos;
        public string Name;
        public bool IsBaseLayer;
        public bool IsWeighted;
        public float PctForNoAsset;
        public List<TerrainGenerationLayeredAssetBaseData> AssetPlacements;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
