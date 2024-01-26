using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Data
{
    public class TerrainGenerationData
    {
        public string GeneratorName; //Must be unique
        public TerrainGenerationTypeBaseData GenerationData;
        public TerrainMeshTypeBaseData MeshData;
    }
}
