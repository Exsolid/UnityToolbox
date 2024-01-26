using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Data
{
    public class TerrainMeshTypeLayeredLayerGroundData: TerrainMeshTypeLayeredLayerBaseData
    {
        public float NoiseCliff;
        public float NoiseCliffGround;
        public float NoiseGround;
        public float Height;
        public List<TerrainGenerationHeightColorData> HeightData;
    }
}
