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
        public abstract void Generate(TerrainGenerationData data, GameObject terrainObject);

    }
}
