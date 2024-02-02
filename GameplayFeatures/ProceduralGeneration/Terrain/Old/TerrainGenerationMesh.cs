using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    public abstract class TerrainGenerationMesh
    {
        public abstract GameObject GenerateMesh(int[,] grid);
    }
}
