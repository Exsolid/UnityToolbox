using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered
{
    /// <summary>
    /// The data for a ground layer of the layered mesh generation type.
    /// </summary>
    public class TerrainMeshTypeLayeredLayerGroundData: TerrainMeshTypeLayeredLayerBaseData
    {
        public float NoiseGround;
        public float Height;
        public TerrainGenerationAssetPosition AssetPositionType;
        public List<TerrainGenerationHeightColorData> HeightData;
    }
}
