using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainMeshTypeLayeredData: TerrainMeshTypeBaseData
    {
        public float SizeBetweenVertices;
        public int VertexMultiplier;
        public float AssetPositionNoise;
        public bool EnabledNormal;
        public bool EnabledEmission;
        public bool EnabledMetallic;
        public bool EnabledOcclusion;
        public bool EnabledRoughness;
        public List<TerrainMeshTypeLayeredLayerBaseData> Layers;
    }
}
