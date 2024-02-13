using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    /// <summary>
    /// The core data for a generator.
    /// </summary>
    public class TerrainGenerationData
    {
        public string GeneratorName;
        public TerrainGenerationTypeBaseData GenerationData;
        public TerrainMeshTypeBaseData MeshData;
    }
}
