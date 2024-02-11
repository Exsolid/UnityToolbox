using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;
using UnityToolbox.General.Algorithms;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    public abstract class TerrainGenerationGenerator
    {
        public Material Mat;

        public abstract void SetData(TerrainGenerationData data, GameObject terrainObject, LayerMask grounLayerMask);
        public abstract void Generate();
        public abstract float GetHighestHeight();
        public abstract float GetLowestHeight();
        public abstract List<TerrainGenerationHeightColorData> CalculateHeights();
    }
}
