using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainMeshTypeLayeredData: TerrainMeshTypeBaseData
    {
        public int FillerVertexCount;
        public List<TerrainMeshTypeLayeredLayerBaseData> Layers;
    }
}
