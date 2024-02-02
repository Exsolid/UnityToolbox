using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainMeshTypeLayeredLayerGroundData: TerrainMeshTypeLayeredLayerBaseData
    {
        public float NoiseGround;
        public float Height;
        public List<TerrainGenerationHeightColorData> HeightData;
    }
}
